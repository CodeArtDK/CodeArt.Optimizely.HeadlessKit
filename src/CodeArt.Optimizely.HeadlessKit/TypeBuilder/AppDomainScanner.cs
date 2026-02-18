using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder
{
    public class AppDomainScanner : IContentTypeProvider, IDisplayTemplateProvider
    {
        private readonly Lazy<List<SaaSContentType>> _contentTypes;
        private readonly Lazy<List<SaaSDisplayTemplate>> _displayTemplates;
        private readonly ILogger<AppDomainScanner> _logger;

        private static readonly Regex ValidGroupNamePattern = new(@"^[a-zA-Z0-9_\-]+$", RegexOptions.Compiled);

        private static readonly HashSet<string> AutoDetectedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "contentReference", "richText", "url"
        };

        public IReadOnlyCollection<SaaSContentType> ContentTypes => _contentTypes.Value;

        public IReadOnlyCollection<SaaSDisplayTemplate> DisplayTemplates => _displayTemplates.Value;

        private static Type UnwrapNullable(Type propertyType)
        {
            return Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        }

        private static Type? GetEnumerableElementType(Type propertyType)
        {
            if (propertyType == typeof(string))
            {
                return null;
            }

            if (propertyType.IsArray)
            {
                return propertyType.GetElementType();
            }

            if (propertyType.IsGenericType)
            {
                var genericDefinition = propertyType.GetGenericTypeDefinition();
                if (genericDefinition == typeof(IEnumerable<>) ||
                    genericDefinition == typeof(ICollection<>) ||
                    genericDefinition == typeof(IList<>) ||
                    genericDefinition == typeof(List<>))
                {
                    return propertyType.GetGenericArguments()[0];
                }
            }

            var enumerableInterface = propertyType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumerableInterface?.GetGenericArguments()[0];
        }

        private string TranslatePropertyTypes(Type propertyType)
        {
            propertyType = UnwrapNullable(propertyType);
            if (GetEnumerableElementType(propertyType) != null)
            {
                return "array";
            }

            switch (propertyType)
            {
                case Type t when t == typeof(string):
                    return "string";
                case Type t when t == typeof(int):
                    return "integer";
                case Type t when t == typeof(bool):
                    return "boolean";
                case Type t when t == typeof(DateTime):
                    return "dateTime";
                case Type t when t == typeof(DateTimeOffset):
                    return "dateTime";
                case Type t when t == typeof(float):
                    return "float";
                case Type t when t == typeof(double):
                    return "float";
                case Type t when t == typeof(decimal):
                    return "float";
                case Type t when t == typeof(GraphContentReference):
                    return "contentReference";
                case Type t when t == typeof(GraphContentRichText):
                    return "richText";
                case Type t when t == typeof(GraphContentUrl):
                    return "url";
                default:
                    return propertyType.Name;
            }
        }

        private string? TranslatePropertyFormat(Type propertyType, CMSPropertyAttribute? attr)
        {
            //Options: selectOne,ShortString, LongString, RichTextString, ContentReference, ListOfString, ImageUrl, DocumentUrl
            if (!string.IsNullOrWhiteSpace(attr?.Format))
            {
                return attr.Format;
            }
            return null;
        }

        private SaaSPropertyItemsDefinition? BuildItemsDefinition(Type propertyType, CMSPropertyAttribute? attr)
        {
            var itemType = GetEnumerableElementType(UnwrapNullable(propertyType));
            var resolvedType = attr?.ItemsType ?? (itemType != null ? TranslatePropertyTypes(itemType) : null);
            if (string.IsNullOrWhiteSpace(resolvedType))
            {
                return null;
            }

            return new SaaSPropertyItemsDefinition
            {
                Type = resolvedType,
                Format = attr?.ItemsFormat,
                AllowedTypes = attr?.ItemsAllowedTypes,
                RestrictedTypes = attr?.ItemsRestrictedTypes
            };
        }

        public AppDomainScanner(ILogger<AppDomainScanner> logger)
        {
            _logger = logger;
            _contentTypes = new Lazy<List<SaaSContentType>>(ScanContentTypes);
            _displayTemplates = new Lazy<List<SaaSDisplayTemplate>>(ScanDisplayTemplates);
        }

        private List<SaaSContentType> ScanContentTypes()
        {
            var result = new List<SaaSContentType>();
            var AllTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException) { return Array.Empty<Type>(); }
            });

            foreach (var t in AllTypes.Where(t =>
            {
                try { return t.GetCustomAttribute(typeof(ContentTypeAttribute), true) != null; }
                catch (TypeLoadException) { return false; }
            }))
            {
                var attr = t.GetCustomAttribute<ContentTypeAttribute>(true);

                // Validate content type key
                if (string.IsNullOrWhiteSpace(attr!.Key))
                {
                    _logger.LogWarning("ContentType on {TypeName} has an empty key — skipping.", t.Name);
                    continue;
                }

                var props = t.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance)
                    .Where(p => p.PropertyType != typeof(GraphContentMetadata)
                             && p.PropertyType != typeof(ContentComposition));
                var properties = props.Select(p => new
                {
                    Info = p,
                    Attr = p.GetCustomAttribute<CMSPropertyAttribute>(),
                    Choices = p.GetCustomAttributes<CMSPropertyChoiceAttribute>(true).ToList()
                }).Select(p =>
                {
                    var propertyType = UnwrapNullable(p.Info.PropertyType);
                    var resolvedType = p.Attr?.Type ?? TranslatePropertyTypes(propertyType);

                    // Validate Format value
                    if (!string.IsNullOrWhiteSpace(p.Attr?.Format) && !PropertyFormats.ValidFormats.Contains(p.Attr.Format))
                    {
                        _logger.LogWarning(
                            "Property {TypeName}.{PropertyName} has unknown Format '{Format}'. Valid formats: {ValidFormats}",
                            t.Name, p.Info.Name, p.Attr.Format, string.Join(", ", PropertyFormats.ValidFormats));
                    }

                    // Validate Group name (alphanumeric, underscore, hyphen only)
                    if (!string.IsNullOrWhiteSpace(p.Attr?.Group) && !ValidGroupNamePattern.IsMatch(p.Attr.Group))
                    {
                        _logger.LogWarning(
                            "Property {TypeName}.{PropertyName} has invalid Group '{Group}'. Group names must contain only alphanumeric characters, underscores, and hyphens.",
                            t.Name, p.Info.Name, p.Attr.Group);
                    }

                    // Warn about redundant Format on auto-detected types
                    if (!string.IsNullOrWhiteSpace(p.Attr?.Format) && AutoDetectedTypes.Contains(resolvedType))
                    {
                        _logger.LogWarning(
                            "Property {TypeName}.{PropertyName} has Format '{Format}' but type '{ResolvedType}' is auto-detected — Format is redundant and will be ignored.",
                            t.Name, p.Info.Name, p.Attr.Format, resolvedType);
                    }

                    var propertyDefinition = new SaaSPropertyDefinition
                    {
                        DisplayName = p.Attr?.DisplayName ?? p.Info.Name,
                        Required = p.Info.GetCustomAttribute<RequiredAttribute>() != null,
                        Type = resolvedType,
                        Localized = p.Info.GetCustomAttribute<CultureSpecificAttribute>() != null,
                        Format = TranslatePropertyFormat(propertyType, p.Attr),
                        SortOrder = p.Attr?.SortOrder ?? 0,
                        Description = p.Attr?.Description,
                        Group = p.Attr?.Group ?? "Information",
                        IndexingType = p.Attr?.IndexingType,
                        AllowedTypes = p.Attr?.AllowedTypes,
                        RestrictedTypes = p.Attr?.RestrictedTypes,
                        ContentType = p.Attr?.ContentType
                    };

                    // Detect DataAnnotations validation attributes
                    var rangeAttr = p.Info.GetCustomAttribute<RangeAttribute>();
                    if (rangeAttr != null)
                    {
                        propertyDefinition.Minimum = rangeAttr.Minimum;
                        propertyDefinition.Maximum = rangeAttr.Maximum;
                    }

                    var maxLengthAttr = p.Info.GetCustomAttribute<MaxLengthAttribute>();
                    if (maxLengthAttr != null)
                    {
                        if (string.Equals(resolvedType, "array", StringComparison.OrdinalIgnoreCase))
                            propertyDefinition.MaxItems = maxLengthAttr.Length;
                        else
                            propertyDefinition.MaxLength = maxLengthAttr.Length;
                    }

                    var minLengthAttr = p.Info.GetCustomAttribute<MinLengthAttribute>();
                    if (minLengthAttr != null)
                    {
                        if (string.Equals(resolvedType, "array", StringComparison.OrdinalIgnoreCase))
                            propertyDefinition.MinItems = minLengthAttr.Length;
                        else
                            propertyDefinition.MinLength = minLengthAttr.Length;
                    }

                    var stringLengthAttr = p.Info.GetCustomAttribute<StringLengthAttribute>();
                    if (stringLengthAttr != null)
                    {
                        propertyDefinition.MaxLength = stringLengthAttr.MaximumLength;
                        if (stringLengthAttr.MinimumLength > 0)
                            propertyDefinition.MinLength = stringLengthAttr.MinimumLength;
                    }

                    var regexAttr = p.Info.GetCustomAttribute<RegularExpressionAttribute>();
                    if (regexAttr != null)
                    {
                        propertyDefinition.Pattern = regexAttr.Pattern;
                    }

                    var dateRangeAttr = p.Info.GetCustomAttribute<CMSDateRangeAttribute>();
                    if (dateRangeAttr != null)
                    {
                        propertyDefinition.Minimum = dateRangeAttr.Minimum;
                        propertyDefinition.Maximum = dateRangeAttr.Maximum;
                    }

                    propertyDefinition.Items = BuildItemsDefinition(propertyType, p.Attr);
                    if (p.Choices.Count != 0)
                    {
                        propertyDefinition.Enum = p.Choices
                            .OrderBy(choice => choice.SortOrder)
                            .Select(choice => new SaaSPropertyEnumValue
                            {
                                DisplayName = choice.DisplayName,
                                Value = choice.Value
                            })
                            .ToList();
                    }

                    return new { k = p.Info.Name, v = propertyDefinition };
                }).ToDictionary(pd => pd.k, pd => pd.v);
                var compositionBehaviors = (attr.CompositionBehaviors ?? new string[0]).ToList();
                if (attr.BaseType == BaseTypes.Element && !compositionBehaviors.Contains("elementEnabled"))
                {
                    compositionBehaviors.Add("elementEnabled");
                }
                else if (attr.BaseType == BaseTypes.Section && !compositionBehaviors.Contains("sectionEnabled"))
                {
                    compositionBehaviors.Add("sectionEnabled");
                }

                SaaSContentType ct = new SaaSContentType()
                {
                    BaseType = attr.BaseType,
                    Description = attr.Description,
                    DisplayName = attr.DisplayName ?? t.Name,
                    Features = attr.Features,
                    Key = attr.Key,
                    SortOrder = attr.SortOrder,
                    Usages = attr.Usages,
                    MediaFileExtensions = attr.MediaFileExtensions ?? new string[0],
                    MayContainTypes = attr.MayContainTypes ?? new string[0],
                    CompositionBehaviors = compositionBehaviors.ToArray(),
                    Contracts = attr.Contracts ?? new string[0],
                    IsContract = attr.IsContract,
                    Source = attr.Source,
                    Properties = properties
                };

                result.Add(ct);
            }

            return result;
        }

        private List<SaaSDisplayTemplate> ScanDisplayTemplates()
        {
            var result = new List<SaaSDisplayTemplate>();
            var AllTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException) { return Array.Empty<Type>(); }
            });

            foreach (var t in AllTypes.Where(t =>
            {
                try { return t.GetCustomAttribute(typeof(DisplayTemplateAttribute), true) != null; }
                catch (TypeLoadException) { return false; }
            }))
            {
                var attr = t.GetCustomAttribute<DisplayTemplateAttribute>(true);
                if (t.IsAssignableTo(typeof(SaaSDisplayTemplate)))
                {
                    var dispTemp = Activator.CreateInstance(t) as SaaSDisplayTemplate;
                    if (dispTemp!.Settings == null)
                    {
                        dispTemp.Settings = new Dictionary<string, SaaSDisplayTemplateSettings>();
                    }

                    if (!string.IsNullOrWhiteSpace(attr?.Key) && string.IsNullOrWhiteSpace(dispTemp.Key))
                    {
                        dispTemp.Key = attr.Key;
                    }

                    if (!string.IsNullOrWhiteSpace(attr?.DisplayName) && string.IsNullOrWhiteSpace(dispTemp.DisplayName))
                    {
                        dispTemp.DisplayName = attr.DisplayName;
                    }

                    if (!string.IsNullOrWhiteSpace(attr?.NodeType) && string.IsNullOrWhiteSpace(dispTemp.NodeType))
                    {
                        dispTemp.NodeType = attr.NodeType;
                    }

                    if (attr?.IsBaseTypeSet == true && dispTemp.BaseType == null)
                    {
                        dispTemp.BaseType = attr.BaseType;
                    }

                    if (!string.IsNullOrWhiteSpace(attr?.ContentType) && string.IsNullOrWhiteSpace(dispTemp.ContentType))
                    {
                        dispTemp.ContentType = attr.ContentType;
                    }

                    if (attr?.IsDefault == true)
                    {
                        dispTemp.IsDefault = true;
                    }

                    // Display template API targets one of: contentType, baseType, or nodeType.
                    // Ensure only one is set; prefer contentType > baseType > nodeType.
                    if (!string.IsNullOrWhiteSpace(dispTemp.ContentType))
                    {
                        dispTemp.NodeType = null;
                        dispTemp.BaseType = null;
                    }
                    else if (dispTemp.BaseType != null)
                    {
                        dispTemp.NodeType = null;
                    }

                    PopulateSettingsFromAttributes(t, dispTemp);
                    result.Add(dispTemp);
                }
            }

            return result;
        }

        private static void PopulateSettingsFromAttributes(Type templateType, SaaSDisplayTemplate template)
        {
            var settingProperties = templateType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => new { Property = p, Setting = p.GetCustomAttribute<DisplayTemplateSettingAttribute>() })
                .Where(x => x.Setting != null)
                .OrderBy(x => x.Setting!.SortOrder);

            foreach (var item in settingProperties)
            {
                var key = item.Property.Name;
                if (template.Settings.ContainsKey(key))
                    continue;

                var choiceAttrs = item.Property.GetCustomAttributes<DisplayTemplateChoiceAttribute>(true)
                    .OrderBy(c => c.SortOrder)
                    .ToList();

                var choices = new Dictionary<string, SaaSDisplayTemplateSettingChoice>();
                foreach (var choice in choiceAttrs)
                {
                    choices[choice.Value] = new SaaSDisplayTemplateSettingChoice
                    {
                        DisplayName = choice.DisplayName,
                        SortOrder = choice.SortOrder > 0 ? choice.SortOrder : null
                    };
                }

                template.Settings[key] = new SaaSDisplayTemplateSettings
                {
                    DisplayName = item.Setting!.DisplayName ?? key,
                    Editor = item.Setting.Editor,
                    SortOrder = item.Setting.SortOrder > 0 ? item.Setting.SortOrder : null,
                    Choices = choices
                };
            }
        }
    }
}

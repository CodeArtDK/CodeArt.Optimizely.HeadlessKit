using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.Text.Json;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Sync
{
    /// <summary>
    /// Compares a local content type definition against a remote one to detect changes
    /// and build a merge-patch payload containing only the changed fields.
    /// </summary>
    public class ContentTypeComparer
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Gets whether differences were detected between the local and remote content types.
        /// </summary>
        public bool HasChanges { get; private set; }

        private readonly SaaSContentType _local;
        private readonly SaaSContentType _remote;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeComparer"/> class and performs comparison.
        /// </summary>
        /// <param name="local">The locally-defined content type.</param>
        /// <param name="remote">The remote content type from the CMS API.</param>
        public ContentTypeComparer(SaaSContentType local, SaaSContentType remote)
        {
            _local = local ?? throw new ArgumentNullException(nameof(local));
            _remote = remote ?? throw new ArgumentNullException(nameof(remote));
            HasChanges = DetectChanges();
        }

        /// <summary>
        /// Returns a content type object containing only the changed fields, suitable for a merge-patch request.
        /// </summary>
        /// <returns>A <see cref="SaaSContentType"/> with only the fields that differ from the remote type.</returns>
        public SaaSContentType BuildPatchPayload()
        {
            if (!HasChanges)
                return new SaaSContentType { Key = _local.Key };

            var patch = new SaaSContentType
            {
                Key = _local.Key,
                BaseType = _local.BaseType
            };

            if (!StringEquals(_local.DisplayName, _remote.DisplayName))
                patch.DisplayName = _local.DisplayName;

            if (!StringEquals(_local.Description, _remote.Description))
                patch.Description = _local.Description;

            if (_local.SortOrder != _remote.SortOrder)
                patch.SortOrder = _local.SortOrder;

            if (_local.IsContract != _remote.IsContract)
                patch.IsContract = _local.IsContract;

            if (_remote.Features is { Length: > 0 } &&
                !ArrayEquals(_local.Features?.Select(f => f.ToString()), _remote.Features.Select(f => f.ToString())))
                patch.Features = _local.Features;

            if (_remote.Usages is { Length: > 0 } &&
                !ArrayEquals(_local.Usages?.Select(u => u.ToString()), _remote.Usages.Select(u => u.ToString())))
                patch.Usages = _local.Usages;

            if (!ArrayEquals(_local.MayContainTypes, _remote.MayContainTypes))
                patch.MayContainTypes = _local.MayContainTypes;

            if (!ArrayEquals(_local.MediaFileExtensions, _remote.MediaFileExtensions))
                patch.MediaFileExtensions = _local.MediaFileExtensions;

            if (!ArrayEquals(_local.CompositionBehaviors, _remote.CompositionBehaviors))
                patch.CompositionBehaviors = _local.CompositionBehaviors;

            if (!ArrayEquals(_local.Contracts, _remote.Contracts))
                patch.Contracts = _local.Contracts;

            // Merge properties: include all local properties (additive, never remove remote-only properties)
            var mergedProperties = new Dictionary<string, SaaSPropertyDefinition>();
            bool propertiesChanged = false;

            foreach (var kvp in _local.Properties)
            {
                if (_remote.Properties.TryGetValue(kvp.Key, out var remoteProp))
                {
                    if (!PropertyEquals(kvp.Value, remoteProp))
                    {
                        mergedProperties[kvp.Key] = kvp.Value;
                        propertiesChanged = true;
                    }
                    else
                    {
                        mergedProperties[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    // New property — include it
                    mergedProperties[kvp.Key] = kvp.Value;
                    propertiesChanged = true;
                }
            }

            // Keep remote-only properties to avoid deletion
            foreach (var kvp in _remote.Properties)
            {
                if (!mergedProperties.ContainsKey(kvp.Key))
                {
                    mergedProperties[kvp.Key] = kvp.Value;
                }
            }

            if (propertiesChanged)
                patch.Properties = mergedProperties;
            else
                patch.Properties = null!; // Exclude from merge-patch to avoid wiping remote properties

            return patch;
        }

        private bool DetectChanges()
        {
            if (!StringEquals(_local.DisplayName, _remote.DisplayName))
                return true;

            if (!StringEquals(_local.Description, _remote.Description))
                return true;

            if (_local.SortOrder != _remote.SortOrder)
                return true;

            if (_local.IsContract != _remote.IsContract)
                return true;

            // Only compare Features/Usages when remote has data; the list endpoint
            // returns empty arrays for these fields so they cannot be compared reliably.
            if (_remote.Features is { Length: > 0 } &&
                !ArrayEquals(_local.Features?.Select(f => f.ToString()), _remote.Features.Select(f => f.ToString())))
                return true;

            if (_remote.Usages is { Length: > 0 } &&
                !ArrayEquals(_local.Usages?.Select(u => u.ToString()), _remote.Usages.Select(u => u.ToString())))
                return true;

            if (!ArrayEquals(_local.MayContainTypes, _remote.MayContainTypes))
                return true;

            if (!ArrayEquals(_local.MediaFileExtensions, _remote.MediaFileExtensions))
                return true;

            if (!ArrayEquals(_local.CompositionBehaviors, _remote.CompositionBehaviors))
                return true;

            if (!ArrayEquals(_local.Contracts, _remote.Contracts))
                return true;

            // Check for new or changed properties
            foreach (var kvp in _local.Properties)
            {
                if (!_remote.Properties.TryGetValue(kvp.Key, out var remoteProp))
                    return true; // New property

                if (!PropertyEquals(kvp.Value, remoteProp))
                    return true;
            }

            return false;
        }

        private static bool PropertyEquals(SaaSPropertyDefinition a, SaaSPropertyDefinition b)
        {
            if (!StringEquals(a.Type, b.Type)) return false;
            if (!StringEquals(a.Format, b.Format)) return false;
            if (!StringEquals(a.DisplayName, b.DisplayName)) return false;
            if (!StringEquals(a.Description, b.Description)) return false;
            if (a.Localized != b.Localized) return false;
            if (a.Required != b.Required) return false;
            if (!StringEquals(a.Group, b.Group)) return false;
            if (a.SortOrder != b.SortOrder) return false;
            if (!StringEquals(a.IndexingType, b.IndexingType)) return false;
            if (!ArrayEquals(a.AllowedTypes, b.AllowedTypes)) return false;
            if (!ArrayEquals(a.RestrictedTypes, b.RestrictedTypes)) return false;
            if (!StringEquals(a.ContentType, b.ContentType)) return false;
            if (!ItemsEquals(a.Items, b.Items)) return false;
            if (a.MinItems != b.MinItems) return false;
            if (a.MaxItems != b.MaxItems) return false;
            if (a.MinLength != b.MinLength) return false;
            if (a.MaxLength != b.MaxLength) return false;
            if (!StringEquals(a.Pattern, b.Pattern)) return false;
            if (!ObjectEquals(a.Minimum, b.Minimum)) return false;
            if (!ObjectEquals(a.Maximum, b.Maximum)) return false;
            if (!EnumValuesEqual(a.Enum, b.Enum)) return false;
            return true;
        }

        private static bool ItemsEquals(SaaSPropertyItemsDefinition? a, SaaSPropertyItemsDefinition? b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (!StringEquals(a.Type, b.Type)) return false;
            if (!StringEquals(a.Format, b.Format)) return false;
            if (!ArrayEquals(a.AllowedTypes, b.AllowedTypes)) return false;
            if (!ArrayEquals(a.RestrictedTypes, b.RestrictedTypes)) return false;
            if (a.MinLength != b.MinLength) return false;
            if (a.MaxLength != b.MaxLength) return false;
            if (!StringEquals(a.Pattern, b.Pattern)) return false;
            return true;
        }

        private static bool EnumValuesEqual(List<SaaSPropertyEnumValue>? a, List<SaaSPropertyEnumValue>? b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!StringEquals(a[i].Value, b[i].Value)) return false;
                if (!StringEquals(a[i].DisplayName, b[i].DisplayName)) return false;
            }
            return true;
        }

        private static bool ObjectEquals(object? a, object? b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            return string.Equals(a.ToString(), b.ToString(), StringComparison.Ordinal);
        }

        private static bool StringEquals(string? a, string? b)
        {
            return string.Equals(a ?? "", b ?? "", StringComparison.Ordinal);
        }

        private static bool ArrayEquals(IEnumerable<string>? a, IEnumerable<string>? b)
        {
            var listA = (a ?? Enumerable.Empty<string>()).ToList();
            var listB = (b ?? Enumerable.Empty<string>()).ToList();

            if (listA.Count != listB.Count) return false;

            return listA.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .SequenceEqual(listB.OrderBy(x => x, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Compares a local display template definition against a remote one to detect changes.
    /// </summary>
    public class DisplayTemplateComparer
    {
        /// <summary>
        /// Gets whether differences were detected between the local and remote display templates.
        /// </summary>
        public bool HasChanges { get; private set; }

        private readonly SaaSDisplayTemplate _local;
        private readonly SaaSDisplayTemplate _remote;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayTemplateComparer"/> class and performs comparison.
        /// </summary>
        /// <param name="local">The locally-defined display template.</param>
        /// <param name="remote">The remote display template from the CMS API.</param>
        public DisplayTemplateComparer(SaaSDisplayTemplate local, SaaSDisplayTemplate remote)
        {
            _local = local ?? throw new ArgumentNullException(nameof(local));
            _remote = remote ?? throw new ArgumentNullException(nameof(remote));
            HasChanges = DetectChanges();
        }

        /// <summary>
        /// Returns the full local display template as the patch payload.
        /// </summary>
        /// <returns>The local <see cref="SaaSDisplayTemplate"/> definition.</returns>
        public SaaSDisplayTemplate BuildPatchPayload()
        {
            // For display templates, send the full local object since they're simpler
            return _local;
        }

        private bool DetectChanges()
        {
            if (!string.Equals(_local.DisplayName ?? "", _remote.DisplayName ?? "", StringComparison.Ordinal))
                return true;
            if (!string.Equals(_local.NodeType ?? "", _remote.NodeType ?? "", StringComparison.Ordinal))
                return true;
            if (_local.BaseType != _remote.BaseType)
                return true;
            if (!string.Equals(_local.ContentType ?? "", _remote.ContentType ?? "", StringComparison.Ordinal))
                return true;
            if (_local.IsDefault != _remote.IsDefault)
                return true;
            if (!SettingsEqual(_local.Settings, _remote.Settings))
                return true;
            return false;
        }

        private static bool SettingsEqual(
            Dictionary<string, SaaSDisplayTemplateSettings>? a,
            Dictionary<string, SaaSDisplayTemplateSettings>? b)
        {
            var dictA = a ?? new Dictionary<string, SaaSDisplayTemplateSettings>();
            var dictB = b ?? new Dictionary<string, SaaSDisplayTemplateSettings>();

            if (dictA.Count != dictB.Count) return false;

            foreach (var kvp in dictA)
            {
                if (!dictB.TryGetValue(kvp.Key, out var bVal))
                    return false;
                if (!string.Equals(kvp.Value.DisplayName, bVal.DisplayName, StringComparison.Ordinal))
                    return false;
                if (!string.Equals(kvp.Value.Editor, bVal.Editor, StringComparison.Ordinal))
                    return false;
                if ((kvp.Value.SortOrder ?? 0) != (bVal.SortOrder ?? 0))
                    return false;
                if (!ChoicesEqual(kvp.Value.Choices, bVal.Choices))
                    return false;
            }

            return true;
        }

        private static bool ChoicesEqual(
            Dictionary<string, SaaSDisplayTemplateSettingChoice>? a,
            Dictionary<string, SaaSDisplayTemplateSettingChoice>? b)
        {
            var dictA = a ?? new Dictionary<string, SaaSDisplayTemplateSettingChoice>();
            var dictB = b ?? new Dictionary<string, SaaSDisplayTemplateSettingChoice>();

            if (dictA.Count != dictB.Count) return false;

            foreach (var kvp in dictA)
            {
                if (!dictB.TryGetValue(kvp.Key, out var bVal))
                    return false;
                if (!string.Equals(kvp.Value.DisplayName, bVal.DisplayName, StringComparison.Ordinal))
                    return false;
                if ((kvp.Value.SortOrder ?? 0) != (bVal.SortOrder ?? 0))
                    return false;
            }

            return true;
        }
    }
}

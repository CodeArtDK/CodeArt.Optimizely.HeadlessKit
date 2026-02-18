using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using CodeArt.Optimizely.HeadlessKit.Mvc.Interfaces;
using CodeArt.Optimizely.HeadlessKit.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    /// <summary>
    /// Discovers and coordinates template mappings between content types and their rendering
    /// templates (Razor Pages, Controllers, ViewComponents). Scans assemblies for
    /// <see cref="TemplateDescriptorAttribute"/> at startup to build the mapping dictionary.
    /// </summary>
    public class TemplateCoordinator : IInitializable
    {
        private Dictionary<Type, List<TemplateMappingInfo>> _templateMappings { get; set; }
        private IEnumerable<EndpointDataSource> _endpointDataSources { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateCoordinator"/> class and scans
        /// all loaded assemblies for <see cref="TemplateDescriptorAttribute"/> to build template mappings.
        /// </summary>
        /// <param name="endpointDataSources">The endpoint data sources used to resolve Razor Page paths.</param>
        public TemplateCoordinator(IEnumerable<EndpointDataSource> endpointDataSources)
        {
            _endpointDataSources = endpointDataSources;

            _templateMappings = new Dictionary<Type, List<TemplateMappingInfo>>();
            //Create template mappings
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(tp => tp.GetCustomAttribute<TemplateDescriptorAttribute>(false) != null)))
            {
                var attr = type.GetCustomAttribute<TemplateDescriptorAttribute>(false);
                var isController = type.IsAssignableTo(typeof(Controller));
                var mapping = new TemplateMappingInfo()
                {
                    TemplateType = type,
                    MappedType = attr!.TemplateFor,
                    AvailableWithoutTag = attr.AvailableWithoutTag,
                    Inherited = attr.Inherited,
                    RenderingTag = attr.RenderingTag,
                    IsEndpoint = type.IsAssignableTo(typeof(PageModel)),
                    IsController = isController
                };
                if (!_templateMappings.ContainsKey(attr.TemplateFor))
                {
                    _templateMappings.Add(attr.TemplateFor, new List<TemplateMappingInfo>());
                }
                _templateMappings[attr.TemplateFor].Add(mapping);
            }
        }

        /// <summary>
        /// Returns the template mapping info for a content type, optionally filtered by rendering tag.
        /// Walks up the type hierarchy if no direct mapping is found.
        /// </summary>
        /// <param name="type">The content type to find a template for.</param>
        /// <param name="renderingTag">Optional rendering tag to filter by.</param>
        /// <param name="component">If <c>true</c>, only returns ViewComponent mappings.</param>
        /// <returns>The matching template mapping info, or <c>null</c> if none is found.</returns>
        public TemplateMappingInfo? GetTemplateMappingInfo(Type type, string? renderingTag = null, bool component = false)
        {


            if (_templateMappings.ContainsKey(type))
            {
                var mappings = _templateMappings[type];
                var mapping = mappings.FirstOrDefault(m => m.MappedType == type && (!component || m.TemplateType.IsSubclassOf(typeof(ViewComponent))) && m.RenderingTag == renderingTag)
                    ?? mappings.FirstOrDefault(m => m.MappedType == type && m.AvailableWithoutTag && (!component || m.TemplateType.IsSubclassOf(typeof(ViewComponent))));
                if (mapping != null) return mapping;
            }
            if (type.BaseType != null && (type.BaseType == typeof(GraphContent) || type.BaseType.IsSubclassOf(typeof(GraphContent)) || typeof(IGraphContent).IsAssignableFrom(type.BaseType)))
            {
                return GetTemplateMappingInfo(type.BaseType, renderingTag, component);
            }
            return null;
        }

        /// <summary>
        /// Gets the Razor Page path for a content type.
        /// </summary>
        /// <param name="type">The content type to find a Razor Page for.</param>
        /// <param name="renderingTag">Optional rendering tag to filter by.</param>
        /// <returns>The Razor Page path, or <c>null</c> if no Razor Page template is registered.</returns>
        public string? GetTemplateEndpointForType(Type type, string? renderingTag = null)
        {
            var mapping = GetTemplateMappingInfo(type, renderingTag);
            if (mapping == null) return null;


            //Find the endpoint that is a razor page with a basetype with the provided type (or a parent type)
            //CompiledActionDescriptor
            var endpoints = _endpointDataSources.First().Endpoints;
            foreach (var endpoint in endpoints)
            {
                var ad = endpoint.Metadata.FirstOrDefault(md => md is CompiledPageActionDescriptor) as CompiledPageActionDescriptor;
                if (ad != null)
                {
                    if (ad.ModelTypeInfo.AsType() == mapping.TemplateType)
                    {
                        mapping.EndpointPath = endpoint.DisplayName;
                        return mapping.EndpointPath;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the ViewComponent type registered for a content type.
        /// </summary>
        /// <param name="type">The content type to find a ViewComponent for.</param>
        /// <param name="renderingTag">Optional rendering tag to filter by.</param>
        /// <returns>The ViewComponent type, or <c>null</c> if no ViewComponent template is registered.</returns>
        public Type? GetComponentForType(Type type, string? renderingTag = null)
        {
            var mapping = GetTemplateMappingInfo(type, renderingTag, true);
            if (mapping == null) return null;
            return mapping.TemplateType;
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {



        }
    }
}

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
    public class TemplateCoordinator : IInitializable
    {
        private Dictionary<Type, List<TemplateMappingInfo>> _templateMappings { get; set; }
        private IEnumerable<EndpointDataSource> _endpointDataSources { get; set; }

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

        public Type? GetComponentForType(Type type, string? renderingTag = null)
        {
            var mapping = GetTemplateMappingInfo(type, renderingTag, true);
            if (mapping == null) return null;
            return mapping.TemplateType;
        }

        public async Task InitializeAsync()
        {



        }
    }
}

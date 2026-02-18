namespace CodeArt.Optimizely.HeadlessKit.Mvc.Models
{
    public class TemplateMappingInfo
    {
        public Type MappedType { get; set; } = default!;

        public Type TemplateType { get; set; } = default!;

        public string? RenderingTag { get; set; }

        public bool AvailableWithoutTag { get; set; }

        public bool Inherited { get; set; }

        public string? EndpointPath { get; set; }

        public bool IsEndpoint { get; set; }

        public bool IsController { get; set; }
    }
}

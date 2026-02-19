using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Blazor.Models.Elements
{
    [ContentType("InfoCardElement", BaseTypes.Element)]
    public class InfoCardElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? Title { get; set; }

        [CultureSpecific]
        public string? Description { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? IconClass { get; set; }

        public GraphContentUrl? Link { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? LinkText { get; set; }
    }
}

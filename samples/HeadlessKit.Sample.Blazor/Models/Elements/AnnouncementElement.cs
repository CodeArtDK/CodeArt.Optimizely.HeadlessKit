using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Blazor.Models.Elements
{
    [ContentType("AnnouncementElement", BaseTypes.Element)]
    public class AnnouncementElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? Title { get; set; }

        [CultureSpecific]
        public string? Body { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? Severity { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? IconClass { get; set; }
    }
}

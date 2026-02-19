using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Blazor.Models.Elements
{
    [ContentType("UsageStatElement", BaseTypes.Element)]
    public class UsageStatElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? Label { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? Unit { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? IconClass { get; set; }

        public int MaxValue { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? DataKey { get; set; }
    }
}

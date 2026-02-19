using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Blazor.Models.Elements
{
    [ContentType("BillingHistoryElement", BaseTypes.Element)]
    public class BillingHistoryElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? Title { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? DateHeader { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? DescriptionHeader { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? AmountHeader { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? StatusHeader { get; set; }

        [CultureSpecific]
        public string? EmptyMessage { get; set; }
    }
}

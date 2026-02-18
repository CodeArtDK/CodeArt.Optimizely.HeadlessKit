using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("PricingElement", BaseTypes.Element)]
    public class PricingElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Price { get; set; }

        [CultureSpecific]
        public string Description { get; set; }

        public GraphContentRichText Features { get; set; }

        [CMSProperty(Format = "shortString")]
        public string ButtonText { get; set; }

        public GraphContentUrl ButtonLink { get; set; }

        public bool Highlighted { get; set; }
    }
}

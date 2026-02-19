using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("BannerElement", BaseTypes.Element)]
    public class BannerElement : GraphBlock
    {
        [CultureSpecific]
        public string Heading { get; set; }

        public GraphContentRichText Body { get; set; }

        public GraphContentReference Image { get; set; }

        public GraphContentUrl Link { get; set; }

        [CMSProperty(Format = "shortString")]
        public string LinkText { get; set; }
    }
}

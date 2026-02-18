using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("CallToActionElement", BaseTypes.Element)]
    public class CallToActionElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        public GraphContentRichText Body { get; set; }

        [CMSProperty(Format = "shortString")]
        public string ButtonText { get; set; }

        public GraphContentUrl ButtonLink { get; set; }

        public GraphContentReference BackgroundImage { get; set; }
    }
}

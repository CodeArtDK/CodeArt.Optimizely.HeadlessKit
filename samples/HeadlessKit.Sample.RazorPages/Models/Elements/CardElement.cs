using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("CardElement", BaseTypes.Element)]
    public class CardElement : GraphBlock
    {
        [CMSProperty(Format = "shortString")]
        public string IconClass { get; set; }

        public GraphContentReference Image { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        [CultureSpecific]
        public string Description { get; set; }

        public GraphContentUrl Link { get; set; }

        [CMSProperty(Format = "shortString")]
        public string LinkText { get; set; }
    }
}

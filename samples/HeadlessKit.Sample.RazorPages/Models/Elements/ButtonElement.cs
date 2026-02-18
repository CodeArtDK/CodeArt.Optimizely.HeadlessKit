using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("ButtonElement", BaseTypes.Element)]
    public class ButtonElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Text { get; set; }

        public GraphContentUrl Link { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Style { get; set; }
    }
}

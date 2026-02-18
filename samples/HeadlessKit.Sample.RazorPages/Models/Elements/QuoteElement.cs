using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("QuoteElement", BaseTypes.Element)]
    public class QuoteElement : GraphBlock
    {
        [CultureSpecific]
        public GraphContentRichText QuoteBody { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Attribution { get; set; }
    }
}

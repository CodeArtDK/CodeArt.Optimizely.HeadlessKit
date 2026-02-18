using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("ArticleListElement", BaseTypes.Element)]
    public class ArticleListElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Heading { get; set; }

        [CMSProperty(DisplayName = "Number of Articles")]
        public int Count { get; set; } = 6;
    }
}

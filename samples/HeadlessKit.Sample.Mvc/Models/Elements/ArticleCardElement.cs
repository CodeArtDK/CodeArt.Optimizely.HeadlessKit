using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("ArticleCardElement", BaseTypes.Element)]
    public class ArticleCardElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        public string Excerpt { get; set; }

        public GraphContentReference Image { get; set; }

        public DateTime Date { get; set; }

        [CMSProperty(Format = "shortString")]
        public string AuthorName { get; set; }

        public GraphContentUrl Link { get; set; }

        [CMSProperty(ItemsFormat = "shortString")]
        public List<string> Tags { get; set; }
    }
}

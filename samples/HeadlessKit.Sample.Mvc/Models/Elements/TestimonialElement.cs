using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("TestimonialElement", BaseTypes.Element)]
    public class TestimonialElement : GraphBlock
    {
        [CultureSpecific]
        public string Quote { get; set; }

        [CMSProperty(Format = "shortString")]
        public string AuthorName { get; set; }

        [CMSProperty(Format = "shortString")]
        public string AuthorTitle { get; set; }

        [CMSProperty(Format = "shortString")]
        public string CompanyName { get; set; }
    }
}

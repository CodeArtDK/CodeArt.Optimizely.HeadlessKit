using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("HtmlBlockElement", BaseTypes.Element)]
    public class HtmlBlockElement : GraphBlock
    {
        [CultureSpecific]
        public GraphContentRichText Body { get; set; }
    }
}

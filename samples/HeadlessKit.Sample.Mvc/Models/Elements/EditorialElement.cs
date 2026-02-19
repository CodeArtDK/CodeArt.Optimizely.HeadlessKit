using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("EditorialElement", CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models.BaseTypes.Element)]
    public class EditorialElement : GraphBlock
    {
        public GraphContentRichText Body { get; set; }
    }
}

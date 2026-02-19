using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("DividerElement", BaseTypes.Element)]
    public class DividerElement : GraphBlock
    {
        [CMSProperty(Format = "shortString")]
        public string Style { get; set; }
    }
}

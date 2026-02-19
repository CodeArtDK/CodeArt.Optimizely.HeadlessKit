using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("AccordionElement", CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models.BaseTypes.Element)]
    public class AccordionElement : GraphBlock
    {
        [CultureSpecific]
        public string Heading { get; set; }

        public GraphContentRichText Body { get; set; }
    }
}

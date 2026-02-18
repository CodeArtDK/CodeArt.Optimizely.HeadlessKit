using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("TagElement", BaseTypes.Element)]
    public class TagElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string TagName { get; set; }

        public GraphContentUrl TagLink { get; set; }
    }
}

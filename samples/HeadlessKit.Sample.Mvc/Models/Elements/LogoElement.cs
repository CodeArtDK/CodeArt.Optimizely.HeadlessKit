using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("LogoElement", BaseTypes.Element)]
    public class LogoElement : GraphBlock
    {
        public GraphContentReference Image { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Name { get; set; }

        public GraphContentUrl Link { get; set; }
    }
}

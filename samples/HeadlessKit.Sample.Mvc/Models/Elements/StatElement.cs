using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("StatElement", BaseTypes.Element)]
    public class StatElement : GraphBlock
    {
        [CMSProperty(Format = "shortString")]
        public string Value { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Label { get; set; }

        [CMSProperty(Format = "shortString")]
        public string IconClass { get; set; }
    }
}

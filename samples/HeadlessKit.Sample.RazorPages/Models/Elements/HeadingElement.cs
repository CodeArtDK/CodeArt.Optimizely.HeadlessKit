using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("HeadingElement", BaseTypes.Element)]
    public class HeadingElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Text { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Level { get; set; }
    }
}

using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("ImageElement", BaseTypes.Element)]
    public class ImageElement : GraphBlock
    {
        [CMSProperty(DisplayName = "Image")]
        public GraphContentReference Image { get; set; }

        [CultureSpecific]
        public string Caption { get; set; }

        [CMSProperty(Format = "shortString")]
        public string AltText { get; set; }
    }
}

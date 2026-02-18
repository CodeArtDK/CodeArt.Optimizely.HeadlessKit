using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("TeaserElement", CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models.BaseTypes.Element, Description = "A teaser component")]
    public class TeaserElement : GraphBlock
    {
        [CultureSpecific]
        public string Heading { get; set; }

        //Image
        public GraphContentReference Image { get; set; }

        //Link
        public GraphContentUrl Link { get; set; }
    }
}

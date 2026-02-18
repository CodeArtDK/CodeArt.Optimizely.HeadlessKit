using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("HeroElement", BaseTypes.Element)]
    public class HeroElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Subtitle { get; set; }

        public GraphContentReference BackgroundImage { get; set; }

        [CMSProperty(Format = "shortString")]
        public string ButtonText { get; set; }

        public GraphContentUrl ButtonLink { get; set; }
    }
}

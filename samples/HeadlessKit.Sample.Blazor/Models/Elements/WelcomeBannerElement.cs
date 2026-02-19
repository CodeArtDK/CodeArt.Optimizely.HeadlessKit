using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Blazor.Models.Elements
{
    [ContentType("WelcomeBannerElement", BaseTypes.Element)]
    public class WelcomeBannerElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? GreetingTemplate { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? Subtitle { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? IconClass { get; set; }
    }
}

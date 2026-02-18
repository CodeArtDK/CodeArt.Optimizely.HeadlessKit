using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Experiences
{
    [ContentType("ListingPage", BaseTypes.Experience, MayContainTypes = new[] {
        "HeroElement", "CardElement", "PersonElement", "TestimonialElement",
        "StatElement", "ContactInfoElement", "AccordionElement", "EditorialElement",
        "TeaserElement", "ArticleCardElement", "TagElement", "BannerElement",
        "ImageElement", "ButtonElement", "CallToActionElement", "QuoteElement",
        "HeadingElement", "VideoElement", "DividerElement", "PricingElement",
        "LogoElement", "FormElement", "HtmlBlockElement", "ArticleListElement",
        "StandardPage", "ArticlePage", "ListingPage", "LandingPage"
    })]
    public class ListingPage : GraphExperience
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string MetaTitle { get; set; }

        [CMSProperty(Format = "shortString")]
        public string MetaDescription { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Heading { get; set; }
    }
}

using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.ComponentModel.DataAnnotations;

namespace HeadlessKit.Sample.RazorPages.Models.Experiences
{
    [ContentType("StandardPage", BaseTypes.Experience, MayContainTypes = new[] {
        "HeroElement", "CardElement", "PersonElement", "TestimonialElement",
        "StatElement", "ContactInfoElement", "AccordionElement", "EditorialElement",
        "TeaserElement", "ArticleCardElement", "TagElement", "BannerElement",
        "ImageElement", "ButtonElement", "CallToActionElement", "QuoteElement",
        "HeadingElement", "VideoElement", "DividerElement", "PricingElement",
        "LogoElement", "FormElement", "HtmlBlockElement", "ArticleListElement",
        "StandardPage", "ArticlePage", "ListingPage", "LandingPage"
    })]
    public class StandardPage : GraphExperience
    {
        [CultureSpecific]
        [MaxLength(200)]
        [CMSProperty(Format = "shortString")]
        public string MetaTitle { get; set; }

        [MaxLength(500)]
        [CMSProperty(Format = "shortString")]
        public string MetaDescription { get; set; }
    }
}

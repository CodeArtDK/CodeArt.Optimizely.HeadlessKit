using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Experiences
{
    [ContentType("LandingPage", BaseTypes.Experience, MayContainTypes = new[] {
        "HeroElement", "CardElement", "PersonElement", "TestimonialElement",
        "StatElement", "ContactInfoElement", "AccordionElement", "EditorialElement",
        "TeaserElement", "ArticleCardElement", "TagElement", "BannerElement",
        "ImageElement", "ButtonElement", "CallToActionElement", "QuoteElement",
        "HeadingElement", "VideoElement", "DividerElement", "PricingElement",
        "LogoElement", "FormElement", "HtmlBlockElement", "ArticleListElement",
        "StandardPage", "ArticlePage", "ListingPage", "LandingPage"
    })]
    public class LandingPage : GraphExperience
    {
        [CMSProperty(DisplayName = "Site Name", Format = "shortString", Group = "SiteSettings")]
        public string? SiteName { get; set; }

        [CMSProperty(DisplayName = "Site Description", Format = "shortString", Group = "SiteSettings")]
        public string? SiteDescription { get; set; }

        [CMSProperty(DisplayName = "Site Logo", Group = "SiteSettings")]
        public GraphContentReference? SiteLogo { get; set; }

        [CMSProperty(DisplayName = "Footer Email", Format = "shortString", Group = "SiteSettings")]
        public string? FooterEmail { get; set; }

        [CMSProperty(DisplayName = "Footer Phone", Format = "shortString", Group = "SiteSettings")]
        public string? FooterPhone { get; set; }

        [CMSProperty(DisplayName = "Twitter URL", Format = "shortString", Group = "SiteSettings")]
        public string? TwitterUrl { get; set; }

        [CMSProperty(DisplayName = "LinkedIn URL", Format = "shortString", Group = "SiteSettings")]
        public string? LinkedInUrl { get; set; }

        [CMSProperty(DisplayName = "GitHub URL", Format = "shortString", Group = "SiteSettings")]
        public string? GitHubUrl { get; set; }

        [CMSProperty(DisplayName = "Copyright Text", Format = "shortString", Group = "SiteSettings")]
        public string? CopyrightText { get; set; }
    }
}

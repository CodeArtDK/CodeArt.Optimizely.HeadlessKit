using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.ComponentModel.DataAnnotations;

namespace HeadlessKit.Sample.RazorPages.Models.Experiences
{
    [ContentType("ArticlePage", BaseTypes.Experience, MayContainTypes = new[] {
        "HeroElement", "CardElement", "PersonElement", "TestimonialElement",
        "StatElement", "ContactInfoElement", "AccordionElement", "EditorialElement",
        "TeaserElement", "ArticleCardElement", "TagElement", "BannerElement",
        "ImageElement", "ButtonElement", "CallToActionElement", "QuoteElement",
        "HeadingElement", "VideoElement", "DividerElement", "PricingElement",
        "LogoElement", "FormElement", "HtmlBlockElement", "ArticleListElement",
        "StandardPage", "ArticlePage", "ListingPage", "LandingPage"
    })]
    public class ArticlePage : GraphExperience
    {
        [CultureSpecific]
        [MaxLength(200)]
        [CMSProperty(Format = "shortString")]
        public string MetaTitle { get; set; }

        [MaxLength(500)]
        [CMSProperty(Format = "shortString")]
        public string MetaDescription { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Excerpt { get; set; }

        public GraphContentReference FeaturedImage { get; set; }

        public GraphContentRichText Body { get; set; }

        public DateTime PublishedDate { get; set; }

        [CMSProperty(Format = "shortString")]
        public string AuthorName { get; set; }

        [CMSProperty(Format = "shortString")]
        public string AuthorRole { get; set; }

        public GraphContentReference AuthorPhoto { get; set; }

        [MaxLength(20)]
        [CMSProperty(ItemsFormat = "shortString")]
        public List<string> Tags { get; set; }
    }
}

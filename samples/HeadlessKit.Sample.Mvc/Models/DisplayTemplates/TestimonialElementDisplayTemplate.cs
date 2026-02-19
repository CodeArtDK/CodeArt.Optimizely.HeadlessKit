using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "TestimonialElementDefault", DisplayName = "Testimonial Element",
        ContentType = "TestimonialElement", IsDefault = true)]
    public class TestimonialElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Style", SortOrder = 10)]
        [DisplayTemplateChoice("card", "Card", SortOrder = 1)]
        [DisplayTemplateChoice("inline", "Inline", SortOrder = 2)]
        [DisplayTemplateChoice("featured", "Featured", SortOrder = 3)]
        public string Style { get; set; } = "card";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Show Quote Icon", SortOrder = 20)]
        [DisplayTemplateChoice("true", "Yes", SortOrder = 1)]
        [DisplayTemplateChoice("false", "No", SortOrder = 2)]
        public string ShowQuoteIcon { get; set; } = "true";
    }
}

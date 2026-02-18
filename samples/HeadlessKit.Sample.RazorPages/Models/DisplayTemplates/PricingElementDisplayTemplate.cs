using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "PricingElementDefault", DisplayName = "Pricing Element",
        ContentType = "PricingElement", IsDefault = true)]
    public class PricingElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Style", SortOrder = 10)]
        [DisplayTemplateChoice("card", "Card", SortOrder = 1)]
        [DisplayTemplateChoice("minimal", "Minimal", SortOrder = 2)]
        public string Style { get; set; } = "card";
    }
}

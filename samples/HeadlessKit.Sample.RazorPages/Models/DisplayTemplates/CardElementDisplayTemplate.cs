using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "CardElementDefault", DisplayName = "Card Element",
        ContentType = "CardElement", IsDefault = true)]
    public class CardElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("vertical", "Vertical", SortOrder = 1)]
        [DisplayTemplateChoice("horizontal", "Horizontal", SortOrder = 2)]
        public string Layout { get; set; } = "vertical";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Image Position", SortOrder = 20)]
        [DisplayTemplateChoice("top", "Top", SortOrder = 1)]
        [DisplayTemplateChoice("left", "Left", SortOrder = 2)]
        [DisplayTemplateChoice("right", "Right", SortOrder = 3)]
        public string ImagePosition { get; set; } = "top";
    }
}

using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "HeroElementDefault", DisplayName = "Hero Element",
        ContentType = "HeroElement", IsDefault = true)]
    public class HeroElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("fullWidth", "Full Width", SortOrder = 1)]
        [DisplayTemplateChoice("contained", "Contained", SortOrder = 2)]
        [DisplayTemplateChoice("split", "Split", SortOrder = 3)]
        public string Layout { get; set; } = "fullWidth";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Height", SortOrder = 20)]
        [DisplayTemplateChoice("large", "Large", SortOrder = 1)]
        [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
        [DisplayTemplateChoice("small", "Small", SortOrder = 3)]
        public string Height { get; set; } = "large";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Text Alignment", SortOrder = 30)]
        [DisplayTemplateChoice("left", "Left", SortOrder = 1)]
        [DisplayTemplateChoice("center", "Center", SortOrder = 2)]
        [DisplayTemplateChoice("right", "Right", SortOrder = 3)]
        public string TextAlignment { get; set; } = "left";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Overlay Opacity", SortOrder = 40)]
        [DisplayTemplateChoice("dark", "Dark", SortOrder = 1)]
        [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
        [DisplayTemplateChoice("light", "Light", SortOrder = 3)]
        [DisplayTemplateChoice("none", "None", SortOrder = 4)]
        public string OverlayOpacity { get; set; } = "dark";
    }
}

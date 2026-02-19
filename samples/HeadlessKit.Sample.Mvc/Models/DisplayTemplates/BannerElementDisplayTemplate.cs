using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "BannerElementDefault", DisplayName = "Banner Element",
        ContentType = "BannerElement", IsDefault = true)]
    public class BannerElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("fullWidth", "Full Width", SortOrder = 1)]
        [DisplayTemplateChoice("contained", "Contained", SortOrder = 2)]
        public string Layout { get; set; } = "fullWidth";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Text Alignment", SortOrder = 20)]
        [DisplayTemplateChoice("left", "Left", SortOrder = 1)]
        [DisplayTemplateChoice("center", "Center", SortOrder = 2)]
        [DisplayTemplateChoice("right", "Right", SortOrder = 3)]
        public string TextAlignment { get; set; } = "left";
    }
}

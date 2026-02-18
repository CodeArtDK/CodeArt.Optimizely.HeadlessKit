using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "DividerElementDefault", DisplayName = "Divider Element",
        ContentType = "DividerElement", IsDefault = true)]
    public class DividerElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Style", SortOrder = 10)]
        [DisplayTemplateChoice("line", "Line", SortOrder = 1)]
        [DisplayTemplateChoice("dashed", "Dashed", SortOrder = 2)]
        [DisplayTemplateChoice("dots", "Dots", SortOrder = 3)]
        [DisplayTemplateChoice("space", "Space", SortOrder = 4)]
        public string Style { get; set; } = "line";
    }
}

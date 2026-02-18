using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "TagElementDefault", DisplayName = "Tag Element",
        ContentType = "TagElement", IsDefault = true)]
    public class TagElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Style", SortOrder = 10)]
        [DisplayTemplateChoice("badge", "Badge", SortOrder = 1)]
        [DisplayTemplateChoice("pill", "Pill", SortOrder = 2)]
        [DisplayTemplateChoice("link", "Link", SortOrder = 3)]
        public string Style { get; set; } = "badge";
    }
}

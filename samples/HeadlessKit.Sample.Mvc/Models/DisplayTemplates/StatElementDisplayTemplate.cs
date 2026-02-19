using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "StatElementDefault", DisplayName = "Stat Element",
        ContentType = "StatElement", IsDefault = true)]
    public class StatElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Size", SortOrder = 10)]
        [DisplayTemplateChoice("large", "Large", SortOrder = 1)]
        [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
        [DisplayTemplateChoice("small", "Small", SortOrder = 3)]
        public string Size { get; set; } = "large";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Show Icon", SortOrder = 20)]
        [DisplayTemplateChoice("true", "Yes", SortOrder = 1)]
        [DisplayTemplateChoice("false", "No", SortOrder = 2)]
        public string ShowIcon { get; set; } = "true";
    }
}

using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "SectionDefault", DisplayName = "Section",
        BaseType = BaseTypes.Section, IsDefault = true)]
    public class SectionDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Color Scheme", SortOrder = 10)]
        [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
        [DisplayTemplateChoice("dark", "Dark", SortOrder = 2)]
        [DisplayTemplateChoice("light", "Light", SortOrder = 3)]
        [DisplayTemplateChoice("accent", "Accent", SortOrder = 4)]
        public string ColorScheme { get; set; } = "default";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Padding", SortOrder = 20)]
        [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
        [DisplayTemplateChoice("compact", "Compact", SortOrder = 2)]
        [DisplayTemplateChoice("spacious", "Spacious", SortOrder = 3)]
        [DisplayTemplateChoice("none", "None", SortOrder = 4)]
        public string Padding { get; set; } = "default";
    }
}

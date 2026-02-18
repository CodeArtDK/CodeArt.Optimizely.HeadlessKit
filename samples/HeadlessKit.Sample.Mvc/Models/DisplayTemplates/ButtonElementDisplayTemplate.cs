using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "ButtonElementDefault", DisplayName = "Button Element",
        ContentType = "ButtonElement", IsDefault = true)]
    public class ButtonElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Size", SortOrder = 10)]
        [DisplayTemplateChoice("large", "Large", SortOrder = 1)]
        [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
        [DisplayTemplateChoice("small", "Small", SortOrder = 3)]
        public string Size { get; set; } = "large";
    }
}

using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "FormElementDefault", DisplayName = "Form Element",
        ContentType = "FormElement", IsDefault = true)]
    public class FormElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Style", SortOrder = 10)]
        [DisplayTemplateChoice("standard", "Standard", SortOrder = 1)]
        [DisplayTemplateChoice("compact", "Compact", SortOrder = 2)]
        public string Style { get; set; } = "standard";
    }
}

using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "AccordionElementSettings", DisplayName = "Accordion Element Settings",
        ContentType = "AccordionElement", IsDefault = true)]
    public class AccordionElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Expanded")]
        [DisplayTemplateChoice("true", "Yes")]
        [DisplayTemplateChoice("false", "No")]
        public string Expanded { get; set; } = "true";
    }
}

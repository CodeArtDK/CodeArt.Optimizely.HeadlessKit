using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "HeadingElementDefault", DisplayName = "Heading Element",
        ContentType = "HeadingElement", IsDefault = true)]
    public class HeadingElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Alignment", SortOrder = 10)]
        [DisplayTemplateChoice("left", "Left", SortOrder = 1)]
        [DisplayTemplateChoice("center", "Center", SortOrder = 2)]
        [DisplayTemplateChoice("right", "Right", SortOrder = 3)]
        public string Alignment { get; set; } = "left";
    }
}

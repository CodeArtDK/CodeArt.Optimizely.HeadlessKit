using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "TeaserElementDefault", DisplayName = "Teaser Element",
        ContentType = "TeaserElement", IsDefault = true)]
    public class TeaserElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("horizontal", "Horizontal", SortOrder = 1)]
        [DisplayTemplateChoice("vertical", "Vertical", SortOrder = 2)]
        public string Layout { get; set; } = "horizontal";
    }
}

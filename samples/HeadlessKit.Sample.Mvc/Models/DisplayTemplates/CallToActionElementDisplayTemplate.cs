using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "CallToActionElementDefault", DisplayName = "Call To Action Element",
        ContentType = "CallToActionElement", IsDefault = true)]
    public class CallToActionElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("centered", "Centered", SortOrder = 1)]
        [DisplayTemplateChoice("split", "Split", SortOrder = 2)]
        public string Layout { get; set; } = "centered";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Background", SortOrder = 20)]
        [DisplayTemplateChoice("image", "Image", SortOrder = 1)]
        [DisplayTemplateChoice("color", "Color", SortOrder = 2)]
        [DisplayTemplateChoice("none", "None", SortOrder = 3)]
        public string Background { get; set; } = "image";
    }
}

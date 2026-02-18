using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "EditorialElementDefault", DisplayName = "Editorial Element",
        ContentType = "EditorialElement", IsDefault = true)]
    public class EditorialElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Width", SortOrder = 10)]
        [DisplayTemplateChoice("full", "Full Width", SortOrder = 1)]
        [DisplayTemplateChoice("narrow", "Narrow", SortOrder = 2)]
        public string Width { get; set; } = "full";
    }
}

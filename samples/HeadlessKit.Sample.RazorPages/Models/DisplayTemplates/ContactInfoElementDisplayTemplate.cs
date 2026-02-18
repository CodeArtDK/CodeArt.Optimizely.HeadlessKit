using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "ContactInfoElementDefault", DisplayName = "Contact Info Element",
        ContentType = "ContactInfoElement", IsDefault = true)]
    public class ContactInfoElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("stacked", "Stacked", SortOrder = 1)]
        [DisplayTemplateChoice("inline", "Inline", SortOrder = 2)]
        [DisplayTemplateChoice("sidebar", "Sidebar", SortOrder = 3)]
        public string Layout { get; set; } = "stacked";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Show Map", SortOrder = 20)]
        [DisplayTemplateChoice("true", "Yes", SortOrder = 1)]
        [DisplayTemplateChoice("false", "No", SortOrder = 2)]
        public string ShowMap { get; set; } = "true";
    }
}

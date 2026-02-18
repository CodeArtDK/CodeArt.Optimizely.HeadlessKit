using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "PersonElementDefault", DisplayName = "Person Element",
        ContentType = "PersonElement", IsDefault = true)]
    public class PersonElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("card", "Card", SortOrder = 1)]
        [DisplayTemplateChoice("inline", "Inline", SortOrder = 2)]
        [DisplayTemplateChoice("detailed", "Detailed", SortOrder = 3)]
        public string Layout { get; set; } = "card";
    }
}

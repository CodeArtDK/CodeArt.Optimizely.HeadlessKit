using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "LogoElementDefault", DisplayName = "Logo Element",
        ContentType = "LogoElement", IsDefault = true)]
    public class LogoElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Size", SortOrder = 10)]
        [DisplayTemplateChoice("large", "Large", SortOrder = 1)]
        [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
        [DisplayTemplateChoice("small", "Small", SortOrder = 3)]
        public string Size { get; set; } = "large";
    }
}

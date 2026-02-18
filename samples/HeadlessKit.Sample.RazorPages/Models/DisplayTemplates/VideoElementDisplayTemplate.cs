using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "VideoElementDefault", DisplayName = "Video Element",
        ContentType = "VideoElement", IsDefault = true)]
    public class VideoElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Size", SortOrder = 10)]
        [DisplayTemplateChoice("full", "Full Width", SortOrder = 1)]
        [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
        [DisplayTemplateChoice("small", "Small", SortOrder = 3)]
        public string Size { get; set; } = "full";
    }
}

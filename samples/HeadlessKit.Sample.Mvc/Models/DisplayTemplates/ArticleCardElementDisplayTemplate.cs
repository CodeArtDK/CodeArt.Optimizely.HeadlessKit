using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "ArticleCardElementDefault", DisplayName = "Article Card Element",
        ContentType = "ArticleCardElement", IsDefault = true)]
    public class ArticleCardElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("vertical", "Vertical", SortOrder = 1)]
        [DisplayTemplateChoice("horizontal", "Horizontal", SortOrder = 2)]
        [DisplayTemplateChoice("compact", "Compact", SortOrder = 3)]
        public string Layout { get; set; } = "vertical";
    }
}

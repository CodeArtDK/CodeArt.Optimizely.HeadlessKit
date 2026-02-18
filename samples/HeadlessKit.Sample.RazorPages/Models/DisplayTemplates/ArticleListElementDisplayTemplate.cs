using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "ArticleListElementDefault", DisplayName = "Article List Element",
        ContentType = "ArticleListElement", IsDefault = true)]
    public class ArticleListElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("grid", "Grid", SortOrder = 1)]
        [DisplayTemplateChoice("list", "List", SortOrder = 2)]
        public string Layout { get; set; } = "grid";
    }
}

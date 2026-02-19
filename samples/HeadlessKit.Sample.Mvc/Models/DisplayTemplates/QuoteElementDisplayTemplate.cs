using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "QuoteElementDefault", DisplayName = "Quote Element",
        ContentType = "QuoteElement", IsDefault = true)]
    public class QuoteElementDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Style", SortOrder = 10)]
        [DisplayTemplateChoice("blockquote", "Block Quote", SortOrder = 1)]
        [DisplayTemplateChoice("pullquote", "Pull Quote", SortOrder = 2)]
        public string Style { get; set; } = "blockquote";
    }
}

using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "RowDefault", DisplayName = "Row",
        NodeType = "row", IsDefault = true)]
    public class RowDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
        [DisplayTemplateChoice("equal", "Equal Columns", SortOrder = 1)]
        [DisplayTemplateChoice("sidebarLeft", "Sidebar Left", SortOrder = 2)]
        [DisplayTemplateChoice("sidebarRight", "Sidebar Right", SortOrder = 3)]
        [DisplayTemplateChoice("wideCenter", "Wide Center", SortOrder = 4)]
        public string Layout { get; set; } = "equal";
    }
}

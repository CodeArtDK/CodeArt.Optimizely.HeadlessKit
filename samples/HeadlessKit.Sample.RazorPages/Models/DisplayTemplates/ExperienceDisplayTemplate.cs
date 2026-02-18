using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.DisplayTemplates
{
    [DisplayTemplate(Key = "ExperienceDefault", DisplayName = "Experience",
        BaseType = BaseTypes.Experience, IsDefault = true)]
    public class ExperienceDisplayTemplate : SaaSDisplayTemplate
    {
        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Color Scheme", SortOrder = 10)]
        [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
        [DisplayTemplateChoice("dark", "Dark", SortOrder = 2)]
        [DisplayTemplateChoice("warm", "Warm", SortOrder = 3)]
        [DisplayTemplateChoice("cool", "Cool", SortOrder = 4)]
        public string ColorScheme { get; set; } = "default";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Typography", SortOrder = 20)]
        [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
        [DisplayTemplateChoice("serif", "Serif", SortOrder = 2)]
        [DisplayTemplateChoice("modern", "Modern", SortOrder = 3)]
        [DisplayTemplateChoice("monospace", "Monospace", SortOrder = 4)]
        public string Typography { get; set; } = "default";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Content Width", SortOrder = 30)]
        [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
        [DisplayTemplateChoice("narrow", "Narrow", SortOrder = 2)]
        [DisplayTemplateChoice("wide", "Wide", SortOrder = 3)]
        [DisplayTemplateChoice("full", "Full Width", SortOrder = 4)]
        public string ContentWidth { get; set; } = "default";

        [JsonIgnore]
        [DisplayTemplateSetting(DisplayName = "Accent Color", SortOrder = 40)]
        [DisplayTemplateChoice("teal", "Teal", SortOrder = 1)]
        [DisplayTemplateChoice("blue", "Blue", SortOrder = 2)]
        [DisplayTemplateChoice("purple", "Purple", SortOrder = 3)]
        [DisplayTemplateChoice("orange", "Orange", SortOrder = 4)]
        public string AccentColor { get; set; } = "teal";
    }
}

using System;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayTemplateSettingAttribute : Attribute
    {
        public string? DisplayName { get; set; }

        public string Editor { get; set; } = "choice";

        public int SortOrder { get; set; }
    }
}

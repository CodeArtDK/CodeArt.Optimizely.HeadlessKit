using System;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DisplayTemplateChoiceAttribute : Attribute
    {
        public DisplayTemplateChoiceAttribute(string value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public string Value { get; }

        public string DisplayName { get; }

        public int SortOrder { get; set; }
    }
}

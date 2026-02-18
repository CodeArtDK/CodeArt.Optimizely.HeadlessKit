using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CMSPropertyAttribute : Attribute
    {
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Group { get; set; }
        public int SortOrder { get; set; }

        public string? Type { get; set; }

        public string? Format { get; set; }

        public string? IndexingType { get; set; }

        public string[]? AllowedTypes { get; set; }

        public string[]? RestrictedTypes { get; set; }

        public string? ContentType { get; set; }

        public string? ItemsType { get; set; }

        public string? ItemsFormat { get; set; }

        public string[]? ItemsAllowedTypes { get; set; }

        public string[]? ItemsRestrictedTypes { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CMSPropertyChoiceAttribute : Attribute
    {
        public CMSPropertyChoiceAttribute(string value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public string Value { get; }

        public string DisplayName { get; }

        public int SortOrder { get; set; }
    }
}

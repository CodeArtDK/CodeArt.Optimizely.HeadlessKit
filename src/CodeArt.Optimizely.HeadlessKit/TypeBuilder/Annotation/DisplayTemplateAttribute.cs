using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DisplayTemplateAttribute : Attribute
    {
        public DisplayTemplateAttribute()
        {
        }

        public DisplayTemplateAttribute(string key)
        {
            Key = key;
        }

        public string? Key { get; set; }

        public string? DisplayName { get; set; }

        public string? NodeType { get; set; }

        /// <summary>
        /// Target base type for this display template (e.g. Section, Experience).
        /// Defaults to an unset sentinel; only applied when explicitly assigned.
        /// </summary>
        public Models.BaseTypes BaseType { get; set; } = (Models.BaseTypes)(-1);

        internal bool IsBaseTypeSet => (int)BaseType >= 0;

        public string? ContentType { get; set; }

        public bool IsDefault { get; set; }
    }
}

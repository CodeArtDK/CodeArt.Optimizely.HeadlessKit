using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    /// <summary>
    /// Marks a class as a display template definition for Optimizely CMS composition rendering.
    /// </summary>
    /// <example>
    /// <code>
    /// [DisplayTemplate(Key = "SectionDefault", DisplayName = "Section", BaseType = BaseTypes.Section, IsDefault = true)]
    /// public class SectionDefaultTemplate
    /// {
    ///     [DisplayTemplateSetting(DisplayName = "Background Color")]
    ///     [DisplayTemplateChoice("white", "White")]
    ///     [DisplayTemplateChoice("gray", "Gray")]
    ///     public string? BackgroundColor { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class)]
    public class DisplayTemplateAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayTemplateAttribute"/> class.
        /// </summary>
        public DisplayTemplateAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayTemplateAttribute"/> class with the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for this display template.</param>
        public DisplayTemplateAttribute(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Gets or sets the unique identifier for this display template.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the label shown in the CMS editor.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the layout element type this template targets (e.g. "Section").
        /// </summary>
        public string? NodeType { get; set; }

        /// <summary>
        /// Gets or sets the target base type for this display template (e.g. Section, Experience).
        /// </summary>
        /// <remarks>
        /// Defaults to an unset sentinel; only applied when explicitly assigned.
        /// </remarks>
        public Models.BaseTypes BaseType { get; set; } = (Models.BaseTypes)(-1);

        internal bool IsBaseTypeSet => (int)BaseType >= 0;

        /// <summary>
        /// Gets or sets the specific content type key this template applies to.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the default template for its target type.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}

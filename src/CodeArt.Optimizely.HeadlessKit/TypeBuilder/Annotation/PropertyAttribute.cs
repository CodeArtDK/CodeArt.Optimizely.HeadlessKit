using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    /// <summary>
    /// Configures CMS property metadata for a content type property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CMSPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the label displayed in the CMS editor.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description shown in the CMS editor.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the property group name for organizing properties in the editor.
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Gets or sets the sort order of this property within its group.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the CMS property type string.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the property format. Use <see cref="PropertyFormats"/> constants for known values.
        /// </summary>
        public string? Format { get; set; }

        /// <summary>
        /// Gets or sets the indexing type for Optimizely Graph search indexing.
        /// </summary>
        public string? IndexingType { get; set; }

        /// <summary>
        /// Gets or sets the content type keys allowed as values for this property.
        /// </summary>
        public string[]? AllowedTypes { get; set; }

        /// <summary>
        /// Gets or sets the content type keys restricted from being used as values for this property.
        /// </summary>
        public string[]? RestrictedTypes { get; set; }

        /// <summary>
        /// Gets or sets the content type key for typed reference properties.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the type string for items in a collection property.
        /// </summary>
        public string? ItemsType { get; set; }

        /// <summary>
        /// Gets or sets the format string for items in a collection property.
        /// </summary>
        public string? ItemsFormat { get; set; }

        /// <summary>
        /// Gets or sets the allowed content type keys for items in a collection property.
        /// </summary>
        public string[]? ItemsAllowedTypes { get; set; }

        /// <summary>
        /// Gets or sets the restricted content type keys for items in a collection property.
        /// </summary>
        public string[]? ItemsRestrictedTypes { get; set; }
    }

    /// <summary>
    /// Defines a selectable value for properties with <c>Format = PropertyFormats.SelectOne</c>.
    /// Apply multiple instances to the same property to define all available choices.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CMSPropertyChoiceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CMSPropertyChoiceAttribute"/> class.
        /// </summary>
        /// <param name="value">The value stored when this choice is selected.</param>
        /// <param name="displayName">The label displayed in the CMS editor.</param>
        public CMSPropertyChoiceAttribute(string value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        /// <summary>
        /// Gets the value stored when this choice is selected.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the label displayed in the CMS editor.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets or sets the sort order of this choice in the selection list.
        /// </summary>
        public int SortOrder { get; set; }
    }
}

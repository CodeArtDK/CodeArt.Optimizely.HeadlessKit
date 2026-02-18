using System;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    /// <summary>
    /// Defines a selectable choice option for a display template setting.
    /// Apply multiple instances to the same property to define all available choices.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DisplayTemplateChoiceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayTemplateChoiceAttribute"/> class.
        /// </summary>
        /// <param name="value">The value stored when this choice is selected.</param>
        /// <param name="displayName">The label displayed in the CMS editor.</param>
        public DisplayTemplateChoiceAttribute(string value, string displayName)
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

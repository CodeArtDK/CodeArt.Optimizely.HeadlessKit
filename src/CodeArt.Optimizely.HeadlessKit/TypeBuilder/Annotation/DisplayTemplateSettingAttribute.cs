using System;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    /// <summary>
    /// Exposes a display template property as an editor-configurable setting in the CMS.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayTemplateSettingAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the label for this setting in the CMS editor.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the editor type used for this setting. Defaults to "choice".
        /// </summary>
        public string Editor { get; set; } = "choice";

        /// <summary>
        /// Gets or sets the sort order of this setting in the editor UI.
        /// </summary>
        public int SortOrder { get; set; }
    }
}

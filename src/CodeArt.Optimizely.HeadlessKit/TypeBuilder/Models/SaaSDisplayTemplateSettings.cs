using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Metadata for a display template setting, including editor type and available choices.
    /// </summary>
    public class SaaSDisplayTemplateSettings
    {
        /// <summary>
        /// Gets or sets the editor-facing display name for this setting.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the editor type used to render this setting in the CMS UI.
        /// </summary>
        [JsonPropertyName("editor")]
        public string? Editor { get; set; }

        /// <summary>
        /// Gets or sets the sort order for this setting.
        /// </summary>
        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the available choices for this setting, keyed by choice value.
        /// </summary>
        [JsonPropertyName("choices")]
        public Dictionary<string, SaaSDisplayTemplateSettingChoice> Choices { get; set; } = new();

    }

    /// <summary>
    /// A choice option within a display template setting.
    /// </summary>
    public class SaaSDisplayTemplateSettingChoice
    {
        /// <summary>
        /// Gets or sets the editor-facing display name for this choice.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the sort order for this choice.
        /// </summary>
        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }
    }
}

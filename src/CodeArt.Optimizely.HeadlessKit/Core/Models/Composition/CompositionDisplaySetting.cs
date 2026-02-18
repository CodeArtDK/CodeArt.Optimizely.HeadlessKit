using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    /// <summary>
    /// Key-value pair for a display setting configured in the CMS editor.
    /// </summary>
    public class CompositionDisplaySetting
    {
        /// <summary>
        /// Gets or sets the setting key.
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the setting value.
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}

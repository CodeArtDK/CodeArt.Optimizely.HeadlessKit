using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    /// <summary>
    /// Leaf node in a composition tree containing an <see cref="IGraphContent"/> component instance.
    /// </summary>
    public class CompositionComponentNode : ICompositionNode
    {
        /// <inheritdoc />
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("nodeType")]
        public string? NodeType { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("layoutType")]
        public string? LayoutType { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("displayTemplateKey")]
        public string? DisplayTemplateKey { get; set; }

        /// <inheritdoc />
        [JsonPropertyName("displaySettings")]
        public List<CompositionDisplaySetting>? DisplaySettings { get; set; }

        /// <summary>
        /// Gets or sets the content component instance rendered by this node.
        /// </summary>
        [JsonPropertyName("component")]
        public IGraphContent? Component { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    /// <summary>
    /// Structural layout node (section, row, or column) in a composition tree.
    /// Contains child nodes that form the nested layout structure.
    /// </summary>
    public class CompositionStructureNode : ICompositionNode
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
        /// Gets or sets the child composition nodes nested within this structural node.
        /// </summary>
        [JsonPropertyName("nodes")]
        public List<ICompositionNode>? Nodes { get; set; }
    }
}

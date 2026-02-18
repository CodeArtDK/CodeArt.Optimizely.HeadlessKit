using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    /// <summary>
    /// Interface for composition tree nodes. Implemented by both structural nodes
    /// (sections, rows, columns) and component nodes (leaf nodes with content).
    /// </summary>
    [JsonConverter(typeof(CompositionNodeJsonConverter))]
    public interface ICompositionNode
    {
        /// <summary>
        /// Gets or sets the node type discriminator (e.g. "CompositionStructureNode", "CompositionComponentNode").
        /// </summary>
        [JsonPropertyName("type")]
        string? Type { get; set; }

        /// <summary>
        /// Gets or sets the semantic node type (e.g. "section", "row", "column").
        /// </summary>
        [JsonPropertyName("nodeType")]
        string? NodeType { get; set; }

        /// <summary>
        /// Gets or sets the layout type for structural nodes.
        /// </summary>
        [JsonPropertyName("layoutType")]
        string? LayoutType { get; set; }

        /// <summary>
        /// Gets or sets the display name of this node.
        /// </summary>
        [JsonPropertyName("displayName")]
        string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the unique key of this composition node.
        /// </summary>
        [JsonPropertyName("key")]
        string? Key { get; set; }

        /// <summary>
        /// Gets or sets the display template key assigned to this node.
        /// </summary>
        [JsonPropertyName("displayTemplateKey")]
        string? DisplayTemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the display settings configured in the CMS editor for this node.
        /// </summary>
        [JsonPropertyName("displaySettings")]
        List<CompositionDisplaySetting>? DisplaySettings { get; set; }
    }
}

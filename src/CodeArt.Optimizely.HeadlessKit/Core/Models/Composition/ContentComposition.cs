using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    /// <summary>
    /// Root composition container representing a Visual Builder page layout.
    /// Contains the tree of structural and component nodes that define the page.
    /// </summary>
    public class ContentComposition
    {
        /// <summary>
        /// Gets or sets the node type of this composition root.
        /// </summary>
        [JsonPropertyName("nodeType")]
        public string? NodeType { get; set; }

        /// <summary>
        /// Gets or sets the display template key assigned to this composition.
        /// </summary>
        [JsonPropertyName("displayTemplateKey")]
        public string? DisplayTemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the display settings configured in the CMS editor.
        /// </summary>
        [JsonPropertyName("displaySettings")]
        public List<CompositionDisplaySetting>? DisplaySettings { get; set; }

        /// <summary>
        /// Gets or sets the child composition nodes (sections, rows, columns, and components).
        /// </summary>
        [JsonPropertyName("nodes")]
        public List<ICompositionNode>? Nodes { get; set; }
    }
}

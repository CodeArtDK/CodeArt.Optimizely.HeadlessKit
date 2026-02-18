using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    public class ContentComposition
    {
        [JsonPropertyName("nodeType")]
        public string? NodeType { get; set; }

        [JsonPropertyName("displayTemplateKey")]
        public string? DisplayTemplateKey { get; set; }

        [JsonPropertyName("displaySettings")]
        public List<CompositionDisplaySetting>? DisplaySettings { get; set; }

        [JsonPropertyName("nodes")]
        public List<ICompositionNode>? Nodes { get; set; }
    }
}

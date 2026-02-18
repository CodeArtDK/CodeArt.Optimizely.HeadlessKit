using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    public class CompositionComponentNode : ICompositionNode
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("nodeType")]
        public string? NodeType { get; set; }

        [JsonPropertyName("layoutType")]
        public string? LayoutType { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("displayTemplateKey")]
        public string? DisplayTemplateKey { get; set; }

        [JsonPropertyName("displaySettings")]
        public List<CompositionDisplaySetting>? DisplaySettings { get; set; }

        [JsonPropertyName("component")]
        public IGraphContent? Component { get; set; }
    }
}

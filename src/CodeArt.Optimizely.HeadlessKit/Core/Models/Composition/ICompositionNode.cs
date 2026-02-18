using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    [JsonConverter(typeof(CompositionNodeJsonConverter))]
    public interface ICompositionNode
    {
        [JsonPropertyName("type")]
        string? Type { get; set; }

        [JsonPropertyName("nodeType")]
        string? NodeType { get; set; }

        [JsonPropertyName("layoutType")]
        string? LayoutType { get; set; }

        [JsonPropertyName("displayName")]
        string? DisplayName { get; set; }

        [JsonPropertyName("key")]
        string? Key { get; set; }

        [JsonPropertyName("displayTemplateKey")]
        string? DisplayTemplateKey { get; set; }

        [JsonPropertyName("displaySettings")]
        List<CompositionDisplaySetting>? DisplaySettings { get; set; }
    }
}

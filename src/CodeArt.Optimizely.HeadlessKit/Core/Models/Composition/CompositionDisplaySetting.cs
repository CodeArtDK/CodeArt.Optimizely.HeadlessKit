using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    public class CompositionDisplaySetting
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}

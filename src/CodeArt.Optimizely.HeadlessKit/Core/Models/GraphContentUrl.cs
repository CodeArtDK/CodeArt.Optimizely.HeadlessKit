using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public class GraphContentUrl
    {
        [JsonPropertyName("base")]
        public string? Base { get; set; }

        [JsonPropertyName("internal")]
        public string? Internal { get; set; }

        [JsonPropertyName("hierarchical")]
        public string? Hierarchical { get; set; }

        [JsonPropertyName("default")]
        public string? Default { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public class GraphContentReference
    {
        [JsonPropertyName("url")]
        public GraphContentUrl? Url { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }

}

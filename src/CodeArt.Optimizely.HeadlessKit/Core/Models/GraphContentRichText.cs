using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public class GraphContentRichText
    {
        [JsonPropertyName("html")]
        public string? Html { get; set; }

        //public dynamic? Json { get; set; }
    }
}

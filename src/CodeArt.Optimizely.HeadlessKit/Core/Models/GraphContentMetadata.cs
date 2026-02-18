using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public class GraphContentMetadata
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("url")]
        public GraphContentUrl? Url { get; set; }

        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }

        [JsonPropertyName("published")]
        public DateTime? Published { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("created")]
        public DateTime? Created { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime? LastModified { get; set; }

        //Instance metadata goes here
        [JsonPropertyName("container")]
        public string? Container { get; set; }
        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }
        [JsonPropertyName("lastModifiedBy")]
        public string? LastModifiedBy { get; set; }
        [JsonPropertyName("locales")]
        public List<string>? Locales { get; set; }
        [JsonPropertyName("routeSegment")]
        public string? RouteSegment { get; set; }

    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    public class SaaSDisplayTemplateListResponse
    {
        [JsonPropertyName("items")]
        public List<SaaSDisplayTemplate> Items { get; set; } = new();

        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalItemCount")]
        public int TotalItemCount { get; set; }
    }
}

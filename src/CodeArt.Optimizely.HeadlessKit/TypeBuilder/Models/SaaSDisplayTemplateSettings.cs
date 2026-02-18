using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    public class SaaSDisplayTemplateSettings
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("editor")]
        public string? Editor { get; set; }

        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }

        [JsonPropertyName("choices")]
        public Dictionary<string, SaaSDisplayTemplateSettingChoice> Choices { get; set; } = new();

    }

    public class SaaSDisplayTemplateSettingChoice
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }
    }
}

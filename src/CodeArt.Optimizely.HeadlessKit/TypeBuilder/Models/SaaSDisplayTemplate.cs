using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    public class SaaSDisplayTemplate
    {
        [JsonPropertyName("key")]
        public virtual string? Key { get; set; }

        [JsonPropertyName("displayName")]
        public virtual string? DisplayName { get; set; }

        [JsonPropertyName("nodeType")]
        public virtual string? NodeType { get; set; }

        [JsonPropertyName("baseType")]
        [JsonConverter(typeof(BaseTypesJsonConverter))]
        public virtual BaseTypes? BaseType
        {
            get; set;
        }

        [JsonPropertyName("contentType")]
        public virtual string? ContentType { get; set; }

        [JsonPropertyName("isDefault")]
        public virtual bool IsDefault { get; set; }

        [JsonPropertyName("settings")]
        public virtual Dictionary<string, SaaSDisplayTemplateSettings> Settings { get; set; } = new Dictionary<string, SaaSDisplayTemplateSettings>();
    }
}

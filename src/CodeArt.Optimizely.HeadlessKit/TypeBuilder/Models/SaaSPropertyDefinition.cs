using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    public class SaaSPropertyDefinition
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } //string, richText, contentReference, array, dateTime, url, float, integer, boolean

        [JsonPropertyName("format")]
        public string? Format { get; set; } //selectOne, shortString, longString, listOfString, imageUrl, documentUrl

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("localized")]
        public bool Localized { get; set; }

        [JsonPropertyName("required")]
        public bool Required { get; set; }

        [JsonPropertyName("group")]
        public string? Group { get; set; }

        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        [JsonPropertyName("indexingType")]
        public string? IndexingType { get; set; }

        [JsonPropertyName("allowedTypes")]
        public string[]? AllowedTypes { get; set; }

        [JsonPropertyName("restrictedTypes")]
        public string[]? RestrictedTypes { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("items")]
        public SaaSPropertyItemsDefinition? Items { get; set; }

        [JsonPropertyName("minItems")]
        public int? MinItems { get; set; }

        [JsonPropertyName("maxItems")]
        public int? MaxItems { get; set; }

        [JsonPropertyName("minLength")]
        public int? MinLength { get; set; }

        [JsonPropertyName("maxLength")]
        public int? MaxLength { get; set; }

        [JsonPropertyName("pattern")]
        public string? Pattern { get; set; }

        [JsonPropertyName("minimum")]
        public object? Minimum { get; set; }

        [JsonPropertyName("maximum")]
        public object? Maximum { get; set; }

        [JsonPropertyName("enum")]
        public List<SaaSPropertyEnumValue>? Enum { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }
    }

    public class SaaSPropertyItemsDefinition
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("format")]
        public string? Format { get; set; }

        [JsonPropertyName("allowedTypes")]
        public string[]? AllowedTypes { get; set; }

        [JsonPropertyName("restrictedTypes")]
        public string[]? RestrictedTypes { get; set; }

        [JsonPropertyName("minLength")]
        public int? MinLength { get; set; }

        [JsonPropertyName("maxLength")]
        public int? MaxLength { get; set; }

        [JsonPropertyName("pattern")]
        public string? Pattern { get; set; }
    }

    public class SaaSChoicePropertyDefinition : SaaSPropertyDefinition
    {
        [JsonPropertyName("enum")]
        public List<SaaSPropertyEnumValue>? Enum { get; set; }
    }

    public class SaaSPropertyEnumValue
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}

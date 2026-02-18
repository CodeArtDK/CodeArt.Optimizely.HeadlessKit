using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    public class SaaSContentType
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("baseType")]
        [JsonConverter(typeof(BaseTypesJsonConverter))]
        public BaseTypes? BaseType { get; set; }

        [JsonPropertyName("isContract")]
        public bool IsContract { get; set; }

        [JsonPropertyName("source")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Source { get; set; }

        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        [JsonPropertyName("features")]
        [JsonConverter(typeof(JsonStringEnumArrayConverter<Features>))]
        public Features[]? Features { get; set; }

        [JsonPropertyName("usage")]
        [JsonConverter(typeof(JsonStringEnumArrayConverter<Usages>))]
        public Usages[]? Usages { get; set; }

        [JsonPropertyName("mayContainTypes")]
        public string[]? MayContainTypes { get; set; }

        [JsonPropertyName("mediaFileExtensions")]
        public string[]? MediaFileExtensions { get; set; }

        [JsonPropertyName("compositionBehaviors")]
        public string[]? CompositionBehaviors { get; set; }

        [JsonPropertyName("contracts")]
        public string[]? Contracts { get; set; }

        [JsonPropertyName("created")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? Created { get; set; }

        [JsonPropertyName("lastModified")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? LastModified { get; set; }

        [JsonPropertyName("lastModifiedBy")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LastModifiedBy { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, SaaSPropertyDefinition> Properties { get; set; } = new();

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }

        public override string ToString()
        {
            return Key ?? base.ToString();
        }
    }
}

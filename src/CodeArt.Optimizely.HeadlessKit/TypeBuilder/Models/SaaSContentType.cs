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
    /// <summary>
    /// Represents a content type definition for the Optimizely SaaS CMS REST API.
    /// </summary>
    public class SaaSContentType
    {
        /// <summary>
        /// Gets or sets the unique content type key.
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the editor-facing display name.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the content type description.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the base type (Page, Block, Media, Experience, etc.).
        /// </summary>
        [JsonPropertyName("baseType")]
        [JsonConverter(typeof(BaseTypesJsonConverter))]
        public BaseTypes? BaseType { get; set; }

        /// <summary>
        /// Gets or sets whether this type is a contract (interface-like type).
        /// </summary>
        [JsonPropertyName("isContract")]
        public bool IsContract { get; set; }

        /// <summary>
        /// Gets or sets the source identifier for this content type.
        /// </summary>
        [JsonPropertyName("source")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the sort order for display in the CMS editor.
        /// </summary>
        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the enabled features for this content type (e.g. localization, versioning).
        /// </summary>
        [JsonPropertyName("features")]
        [JsonConverter(typeof(JsonStringEnumArrayConverter<Features>))]
        public Features[]? Features { get; set; }

        /// <summary>
        /// Gets or sets the allowed usages for this content type (e.g. property, listItem).
        /// </summary>
        [JsonPropertyName("usage")]
        [JsonConverter(typeof(JsonStringEnumArrayConverter<Usages>))]
        public Usages[]? Usages { get; set; }

        /// <summary>
        /// Gets or sets the content type keys that may be contained within this type's content areas.
        /// </summary>
        [JsonPropertyName("mayContainTypes")]
        public string[]? MayContainTypes { get; set; }

        /// <summary>
        /// Gets or sets the allowed media file extensions for media content types.
        /// </summary>
        [JsonPropertyName("mediaFileExtensions")]
        public string[]? MediaFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets the composition behaviors enabled for this content type.
        /// </summary>
        [JsonPropertyName("compositionBehaviors")]
        public string[]? CompositionBehaviors { get; set; }

        /// <summary>
        /// Gets or sets the contract keys that this content type implements.
        /// </summary>
        [JsonPropertyName("contracts")]
        public string[]? Contracts { get; set; }

        /// <summary>
        /// Gets or sets when this content type was created. Read-only from the API.
        /// </summary>
        [JsonPropertyName("created")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? Created { get; set; }

        /// <summary>
        /// Gets or sets when this content type was last modified. Read-only from the API.
        /// </summary>
        [JsonPropertyName("lastModified")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        /// Gets or sets who last modified this content type. Read-only from the API.
        /// </summary>
        [JsonPropertyName("lastModifiedBy")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the property definitions for this content type, keyed by property name.
        /// </summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, SaaSPropertyDefinition> Properties { get; set; } = new();

        /// <summary>
        /// Gets or sets any additional JSON properties not mapped to strongly-typed members.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }

        /// <summary>
        /// Returns the content type key.
        /// </summary>
        public override string ToString()
        {
            return Key ?? base.ToString();
        }
    }
}

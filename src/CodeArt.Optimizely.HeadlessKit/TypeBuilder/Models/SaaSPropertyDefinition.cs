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
    /// <summary>
    /// Represents a property definition within a SaaS content type.
    /// </summary>
    public class SaaSPropertyDefinition
    {
        /// <summary>
        /// Gets or sets the property type (e.g. "string", "richText", "contentReference", "array", "dateTime", "url", "float", "integer", "boolean").
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; } //string, richText, contentReference, array, dateTime, url, float, integer, boolean

        /// <summary>
        /// Gets or sets the property format (e.g. "selectOne", "shortString", "longString", "listOfString", "imageUrl", "documentUrl").
        /// </summary>
        [JsonPropertyName("format")]
        public string? Format { get; set; } //selectOne, shortString, longString, listOfString, imageUrl, documentUrl

        /// <summary>
        /// Gets or sets the editor-facing display name.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the property description shown in the editor.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether this property is localizable (culture-specific).
        /// </summary>
        [JsonPropertyName("localized")]
        public bool Localized { get; set; }

        /// <summary>
        /// Gets or sets whether this property is required.
        /// </summary>
        [JsonPropertyName("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the editor group this property belongs to.
        /// </summary>
        [JsonPropertyName("group")]
        public string? Group { get; set; }

        /// <summary>
        /// Gets or sets the sort order within its group.
        /// </summary>
        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the indexing type for search (e.g. "searchable", "queryable").
        /// </summary>
        [JsonPropertyName("indexingType")]
        public string? IndexingType { get; set; }

        /// <summary>
        /// Gets or sets the allowed content type keys for content reference or content area properties.
        /// </summary>
        [JsonPropertyName("allowedTypes")]
        public string[]? AllowedTypes { get; set; }

        /// <summary>
        /// Gets or sets the restricted content type keys for content reference or content area properties.
        /// </summary>
        [JsonPropertyName("restrictedTypes")]
        public string[]? RestrictedTypes { get; set; }

        /// <summary>
        /// Gets or sets the content type key for typed properties (e.g. component properties).
        /// </summary>
        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the items definition for array-type properties.
        /// </summary>
        [JsonPropertyName("items")]
        public SaaSPropertyItemsDefinition? Items { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of items for array-type properties.
        /// </summary>
        [JsonPropertyName("minItems")]
        public int? MinItems { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items for array-type properties.
        /// </summary>
        [JsonPropertyName("maxItems")]
        public int? MaxItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum string length.
        /// </summary>
        [JsonPropertyName("minLength")]
        public int? MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum string length.
        /// </summary>
        [JsonPropertyName("maxLength")]
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a regex pattern for string validation.
        /// </summary>
        [JsonPropertyName("pattern")]
        public string? Pattern { get; set; }

        /// <summary>
        /// Gets or sets the minimum value for numeric properties.
        /// </summary>
        [JsonPropertyName("minimum")]
        public object? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum value for numeric properties.
        /// </summary>
        [JsonPropertyName("maximum")]
        public object? Maximum { get; set; }

        /// <summary>
        /// Gets or sets the enumeration values for select-type properties.
        /// </summary>
        [JsonPropertyName("enum")]
        public List<SaaSPropertyEnumValue>? Enum { get; set; }

        /// <summary>
        /// Gets or sets any additional JSON properties not mapped to strongly-typed members.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }
    }

    /// <summary>
    /// Defines the item schema for array-type property definitions.
    /// </summary>
    public class SaaSPropertyItemsDefinition
    {
        /// <summary>
        /// Gets or sets the item type.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the item format.
        /// </summary>
        [JsonPropertyName("format")]
        public string? Format { get; set; }

        /// <summary>
        /// Gets or sets the allowed content type keys for items.
        /// </summary>
        [JsonPropertyName("allowedTypes")]
        public string[]? AllowedTypes { get; set; }

        /// <summary>
        /// Gets or sets the restricted content type keys for items.
        /// </summary>
        [JsonPropertyName("restrictedTypes")]
        public string[]? RestrictedTypes { get; set; }

        /// <summary>
        /// Gets or sets the minimum string length for items.
        /// </summary>
        [JsonPropertyName("minLength")]
        public int? MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum string length for items.
        /// </summary>
        [JsonPropertyName("maxLength")]
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a regex pattern for item string validation.
        /// </summary>
        [JsonPropertyName("pattern")]
        public string? Pattern { get; set; }
    }

    /// <summary>
    /// Extends <see cref="SaaSPropertyDefinition"/> with enumeration values for choice-based properties.
    /// </summary>
    public class SaaSChoicePropertyDefinition : SaaSPropertyDefinition
    {
        /// <summary>
        /// Gets or sets the enumeration values for this choice property.
        /// </summary>
        [JsonPropertyName("enum")]
        public List<SaaSPropertyEnumValue>? Enum { get; set; }
    }

    /// <summary>
    /// Represents a single enumeration value within a choice property.
    /// </summary>
    public class SaaSPropertyEnumValue
    {
        /// <summary>
        /// Gets or sets the display name shown in the editor.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the stored value.
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}

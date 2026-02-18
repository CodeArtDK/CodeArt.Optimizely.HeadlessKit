using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Metadata associated with a content item from Optimizely Graph.
    /// </summary>
    public class GraphContentMetadata
    {
        /// <summary>
        /// Gets or sets the unique content identifier.
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the content locale (e.g. "en", "sv").
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        /// <summary>
        /// Gets or sets the content version identifier.
        /// </summary>
        [JsonPropertyName("version")]
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the editor-facing display name.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the content URL representations.
        /// </summary>
        [JsonPropertyName("url")]
        public GraphContentUrl? Url { get; set; }

        /// <summary>
        /// Gets or sets the content type hierarchy (list of type names from most specific to base).
        /// </summary>
        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }

        /// <summary>
        /// Gets or sets the publication date.
        /// </summary>
        [JsonPropertyName("published")]
        public DateTime? Published { get; set; }

        /// <summary>
        /// Gets or sets the publication status (e.g. "Published", "Draft").
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [JsonPropertyName("created")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        [JsonPropertyName("lastModified")]
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the container identifier for this content item.
        /// </summary>
        [JsonPropertyName("container")]
        public string? Container { get; set; }

        /// <summary>
        /// Gets or sets the user who created this content item.
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified this content item.
        /// </summary>
        [JsonPropertyName("lastModifiedBy")]
        public string? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the list of available locales for this content item.
        /// </summary>
        [JsonPropertyName("locales")]
        public List<string>? Locales { get; set; }

        /// <summary>
        /// Gets or sets the URL route segment for this content item.
        /// </summary>
        [JsonPropertyName("routeSegment")]
        public string? RouteSegment { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Represents a display template definition for the Optimizely SaaS CMS REST API.
    /// </summary>
    public class SaaSDisplayTemplate
    {
        /// <summary>
        /// Gets or sets the unique template key.
        /// </summary>
        [JsonPropertyName("key")]
        public virtual string? Key { get; set; }

        /// <summary>
        /// Gets or sets the editor-facing display name.
        /// </summary>
        [JsonPropertyName("displayName")]
        public virtual string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the node type this template applies to.
        /// </summary>
        [JsonPropertyName("nodeType")]
        public virtual string? NodeType { get; set; }

        /// <summary>
        /// Gets or sets the base type this template is associated with.
        /// </summary>
        [JsonPropertyName("baseType")]
        [JsonConverter(typeof(BaseTypesJsonConverter))]
        public virtual BaseTypes? BaseType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the content type key this template is associated with.
        /// </summary>
        [JsonPropertyName("contentType")]
        public virtual string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets whether this is the default template for its content type.
        /// </summary>
        [JsonPropertyName("isDefault")]
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the template settings, keyed by setting name.
        /// </summary>
        [JsonPropertyName("settings")]
        public virtual Dictionary<string, SaaSDisplayTemplateSettings> Settings { get; set; } = new Dictionary<string, SaaSDisplayTemplateSettings>();
    }
}

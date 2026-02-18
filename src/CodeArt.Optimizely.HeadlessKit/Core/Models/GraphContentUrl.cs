using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Represents content URL with multiple representations from Optimizely Graph.
    /// </summary>
    public class GraphContentUrl
    {
        /// <summary>
        /// Gets or sets the base URL (scheme and host).
        /// </summary>
        [JsonPropertyName("base")]
        public string? Base { get; set; }

        /// <summary>
        /// Gets or sets the internal CMS URL.
        /// </summary>
        [JsonPropertyName("internal")]
        public string? Internal { get; set; }

        /// <summary>
        /// Gets or sets the full hierarchical URL path.
        /// </summary>
        [JsonPropertyName("hierarchical")]
        public string? Hierarchical { get; set; }

        /// <summary>
        /// Gets or sets the default URL path.
        /// </summary>
        [JsonPropertyName("default")]
        public string? Default { get; set; }

        /// <summary>
        /// Gets or sets the URL type.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Represents a reference to content (e.g. an image or linked page).
    /// Contains a nested URL and optional metadata.
    /// </summary>
    public class GraphContentReference
    {
        /// <summary>
        /// Gets or sets the URL of the referenced content.
        /// </summary>
        [JsonPropertyName("url")]
        public GraphContentUrl? Url { get; set; }

        /// <summary>
        /// Gets or sets the unique key (GUID) of the referenced content.
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }

}

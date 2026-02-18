using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Represents rich text content from Optimizely Graph.
    /// </summary>
    public class GraphContentRichText
    {
        /// <summary>
        /// Gets or sets the rendered HTML string.
        /// </summary>
        [JsonPropertyName("html")]
        public string? Html { get; set; }

        //public dynamic? Json { get; set; }
    }
}

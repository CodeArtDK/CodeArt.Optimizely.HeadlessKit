using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Base class for all content types returned from Optimizely Graph.
    /// Provides metadata and serves as the root of the content type hierarchy.
    /// </summary>
    public class GraphContent : IGraphContent
    {
        /// <summary>
        /// Gets or sets the content metadata from Optimizely Graph.
        /// </summary>
        [JsonPropertyName("_metadata")]
        public GraphContentMetadata? MetaData { get; set; }

        //_deleted, _id, _link_ modified _score
    }

}

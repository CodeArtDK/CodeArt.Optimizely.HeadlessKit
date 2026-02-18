using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public class GraphContent : IGraphContent
    {
        [JsonPropertyName("_metadata")]
        public GraphContentMetadata? MetaData { get; set; }

        //_deleted, _id, _link_ modified _score
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public class GraphResponse<ItemType> where ItemType : class, IGraphContent
    {
        [JsonPropertyName("_Content")]
        public GraphContentWrapper<ItemType>? Content { get; set; }
    }

    public class GraphContentWrapper<ItemType> where ItemType : class, IGraphContent
    {
        [JsonPropertyName("items")]
        public List<ItemType>? Items { get; set; }
    }
}

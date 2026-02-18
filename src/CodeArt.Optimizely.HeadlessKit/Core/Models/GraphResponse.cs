using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Raw response wrapper from GraphQL content queries.
    /// </summary>
    /// <typeparam name="ItemType">The content type of the items in the response.</typeparam>
    public class GraphResponse<ItemType> where ItemType : class, IGraphContent
    {
        /// <summary>
        /// Gets or sets the content wrapper containing the query result items.
        /// </summary>
        [JsonPropertyName("_Content")]
        public GraphContentWrapper<ItemType>? Content { get; set; }
    }

    /// <summary>
    /// Wrapper for the items collection within a <see cref="GraphResponse{ItemType}"/>.
    /// </summary>
    /// <typeparam name="ItemType">The content type of the items.</typeparam>
    public class GraphContentWrapper<ItemType> where ItemType : class, IGraphContent
    {
        /// <summary>
        /// Gets or sets the list of content items returned by the query.
        /// </summary>
        [JsonPropertyName("items")]
        public List<ItemType>? Items { get; set; }
    }
}

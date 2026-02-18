using CodeArt.Optimizely.HeadlessKit.Core.Models;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Contains the result of a full-text search query, extending <see cref="GraphQueryResult{T}"/>
    /// with optional facet aggregations and autocomplete suggestions.
    /// </summary>
    /// <typeparam name="T">The content model type.</typeparam>
    public class SearchResult<T> : GraphQueryResult<T> where T : class, IGraphContent
    {
        /// <summary>Gets or sets the aggregated facet results, if facets were requested.</summary>
        public List<FacetResult>? Facets { get; set; }

        /// <summary>Gets or sets autocomplete suggestions, if available.</summary>
        public List<string>? Autocomplete { get; set; }
    }

    /// <summary>
    /// Represents a single facet value with its aggregated count.
    /// </summary>
    public class FacetResult
    {
        /// <summary>Gets or sets the facet value name.</summary>
        public string? Name { get; set; }

        /// <summary>Gets or sets the number of content items matching this facet value.</summary>
        public int Count { get; set; }
    }
}

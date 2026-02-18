using CodeArt.Optimizely.HeadlessKit.Core.Models;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public class SearchResult<T> : GraphQueryResult<T> where T : class, IGraphContent
    {
        public List<FacetResult>? Facets { get; set; }
        public List<string>? Autocomplete { get; set; }
    }

    public class FacetResult
    {
        public string? Name { get; set; }
        public int Count { get; set; }
    }
}

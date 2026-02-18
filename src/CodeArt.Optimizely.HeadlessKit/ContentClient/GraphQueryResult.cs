using CodeArt.Optimizely.HeadlessKit.Core.Models;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Contains the result of a GraphQL content query, including the matched items,
    /// total count, and pagination cursor.
    /// </summary>
    /// <typeparam name="T">The content model type.</typeparam>
    public class GraphQueryResult<T> where T : class, IGraphContent
    {
        /// <summary>Gets or sets the matched content items for the current page.</summary>
        public List<T> Items { get; set; } = new();

        /// <summary>Gets or sets the total number of matching items (may exceed <see cref="Items"/> count due to pagination).</summary>
        public int Total { get; set; }

        /// <summary>Gets or sets the pagination cursor for fetching the next page. <c>null</c> if no more pages.</summary>
        public string? Cursor { get; set; }
    }
}

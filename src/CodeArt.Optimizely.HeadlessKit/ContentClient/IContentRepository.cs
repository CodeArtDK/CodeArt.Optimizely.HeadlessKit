using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Provides cached content retrieval from Optimizely Graph.
    /// Implementations may cache results to reduce GraphQL round-trips.
    /// </summary>
    public interface IContentRepository
    {
        /// <summary>
        /// Gets child content items for a parent content key.
        /// </summary>
        /// <typeparam name="CT">The content model type to deserialize children into.</typeparam>
        /// <param name="parentkey">The content key of the parent item.</param>
        /// <param name="locale">Optional locale filter.</param>
        /// <returns>A list of child content items, or <c>null</c> if the parent is not found.</returns>
        Task<List<CT>?> GetChildren<CT>(string parentkey, string[]? locale = null) where CT : class, IGraphContent;

        /// <summary>
        /// Gets a content item by its content key.
        /// </summary>
        /// <typeparam name="CT">The content model type to deserialize into.</typeparam>
        /// <param name="key">The unique content key.</param>
        /// <param name="locale">Optional locale filter.</param>
        /// <returns>The content item, or <c>null</c> if not found.</returns>
        Task<CT?> GetContent<CT>(string key, string[]? locale = null) where CT : class, IGraphContent;

        /// <summary>
        /// Gets a content item by its URL path.
        /// </summary>
        /// <typeparam name="CT">The content model type to deserialize into.</typeparam>
        /// <param name="path">The URL path of the content item.</param>
        /// <param name="locale">Optional locale filter.</param>
        /// <returns>The content item, or <c>null</c> if not found.</returns>
        Task<CT?> GetContentByPath<CT>(string path, string[]? locale = null) where CT : class, IGraphContent;
    }
}

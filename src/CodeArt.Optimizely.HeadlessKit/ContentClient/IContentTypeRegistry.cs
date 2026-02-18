using CodeArt.Optimizely.HeadlessKit.Core.Models;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Discovers and resolves content types at runtime. Provides collections of
    /// page types, component types, and all content types, as well as name-based type resolution.
    /// </summary>
    public interface IContentTypeRegistry
    {
        /// <summary>
        /// Gets the collection of types implementing <see cref="IGraphPageContent"/>.
        /// </summary>
        IReadOnlyCollection<Type> PageTypes { get; }

        /// <summary>
        /// Gets the collection of types implementing <see cref="IGraphComponentContent"/>.
        /// </summary>
        IReadOnlyCollection<Type> ComponentTypes { get; }

        /// <summary>
        /// Gets all discovered <see cref="IGraphContent"/> types.
        /// </summary>
        IReadOnlyCollection<Type> AllTypes { get; }

        /// <summary>
        /// Resolves a .NET type by its name. The lookup is case-insensitive.
        /// </summary>
        /// <param name="typeName">The type name to resolve (e.g. "StandardPage").</param>
        /// <returns>The resolved <see cref="Type"/>, or <c>null</c> if not found.</returns>
        Type? ResolveType(string typeName);
    }
}

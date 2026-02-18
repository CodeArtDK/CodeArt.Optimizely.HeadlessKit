using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.Collections.Generic;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder
{
    /// <summary>
    /// Provides discovered content type definitions from assembly scanning.
    /// </summary>
    public interface IContentTypeProvider
    {
        /// <summary>
        /// Gets the collection of content type definitions discovered from annotated classes.
        /// </summary>
        IReadOnlyCollection<SaaSContentType> ContentTypes { get; }
    }

    /// <summary>
    /// Provides discovered display template definitions from assembly scanning.
    /// </summary>
    public interface IDisplayTemplateProvider
    {
        /// <summary>
        /// Gets the collection of display template definitions discovered from annotated classes.
        /// </summary>
        IReadOnlyCollection<SaaSDisplayTemplate> DisplayTemplates { get; }
    }
}

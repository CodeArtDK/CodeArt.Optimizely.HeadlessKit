using CodeArt.Optimizely.HeadlessKit.ContentClient;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    /// <summary>
    /// Base class for view components that need access to <see cref="IContentRepository"/>
    /// for fetching additional content from Optimizely Graph.
    /// </summary>
    public abstract class ContentViewComponentBase : ViewComponent
    {
        /// <summary>
        /// The injected content repository for querying content from Optimizely Graph.
        /// </summary>
        protected IContentRepository ContentRepository { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentViewComponentBase"/> class.
        /// </summary>
        /// <param name="contentRepository">The content repository to inject.</param>
        protected ContentViewComponentBase(IContentRepository contentRepository)
        {
            ContentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        }
    }
}

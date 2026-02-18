using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Controllers
{
    /// <summary>
    /// Generic base controller for MVC content rendering. Provides access to
    /// <see cref="ContentRepository"/> and a virtual <see cref="Index"/> action that renders
    /// the current content model.
    /// </summary>
    /// <typeparam name="TContent">The content type this controller handles.</typeparam>
    public abstract class ContentControllerBase<TContent> : Controller where TContent : class, IGraphContent
    {
        private IContentRepository? _contentRepository;

        /// <summary>
        /// The content repository for querying additional content. Resolved from DI on first access.
        /// </summary>
        protected IContentRepository ContentRepository =>
            _contentRepository ??= HttpContext.RequestServices.GetService(typeof(IContentRepository)) as IContentRepository
            ?? throw new InvalidOperationException("IContentRepository is not registered in the service container.");

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentControllerBase{TContent}"/> class.
        /// The <see cref="ContentRepository"/> will be resolved lazily from the request services.
        /// </summary>
        protected ContentControllerBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentControllerBase{TContent}"/> class
        /// with an explicitly provided content repository.
        /// </summary>
        /// <param name="contentRepository">The content repository to use.</param>
        protected ContentControllerBase(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        }

        /// <summary>
        /// Default action that renders the content item. Override for custom rendering logic.
        /// </summary>
        /// <param name="CurrentContent">The content item resolved by the content routing infrastructure.</param>
        /// <returns>A view result rendering the content.</returns>
        public virtual async Task<IActionResult> Index(TContent CurrentContent)
        {
            return View(CurrentContent);
        }
    }
}

using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Controllers
{
    public abstract class ContentControllerBase<TContent> : Controller where TContent : class, IGraphContent
    {
        private IContentRepository? _contentRepository;

        protected IContentRepository ContentRepository =>
            _contentRepository ??= HttpContext.RequestServices.GetService(typeof(IContentRepository)) as IContentRepository
            ?? throw new InvalidOperationException("IContentRepository is not registered in the service container.");

        protected ContentControllerBase()
        {
        }

        protected ContentControllerBase(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        }

        public virtual async Task<IActionResult> Index(TContent CurrentContent)
        {
            return View(CurrentContent);
        }
    }
}

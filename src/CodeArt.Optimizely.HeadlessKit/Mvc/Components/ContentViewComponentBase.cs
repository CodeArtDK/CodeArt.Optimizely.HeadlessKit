using CodeArt.Optimizely.HeadlessKit.ContentClient;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    public abstract class ContentViewComponentBase : ViewComponent
    {
        protected IContentRepository ContentRepository { get; }

        protected ContentViewComponentBase(IContentRepository contentRepository)
        {
            ContentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        }
    }
}

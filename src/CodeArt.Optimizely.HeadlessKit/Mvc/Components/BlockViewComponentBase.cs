using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    public abstract class BlockViewComponentBase : ViewComponent
    {

    }

    public abstract class BlockViewComponentBase<TBlock> : BlockViewComponentBase where TBlock : class, IGraphContent
    {
        protected IContentRepository ContentRepository { get; }

        protected BlockViewComponentBase(IContentRepository contentRepository)
        {
            ContentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        }

        public abstract Task<IViewComponentResult> InvokeAsync(TBlock model);
    }
}

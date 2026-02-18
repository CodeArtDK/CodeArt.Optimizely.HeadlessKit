using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    /// <summary>
    /// Non-generic base class for view components that render content blocks.
    /// Derive from <see cref="BlockViewComponentBase{TBlock}"/> for typed block rendering.
    /// </summary>
    public abstract class BlockViewComponentBase : ViewComponent
    {

    }

    /// <summary>
    /// Typed base class for view components that render a specific block or element type.
    /// Provides access to <see cref="ContentRepository"/> for fetching additional content.
    /// </summary>
    /// <typeparam name="TBlock">The content block type this view component renders.</typeparam>
    public abstract class BlockViewComponentBase<TBlock> : BlockViewComponentBase where TBlock : class, IGraphContent
    {
        /// <summary>
        /// The content repository for querying additional content from Optimizely Graph.
        /// </summary>
        protected IContentRepository ContentRepository { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockViewComponentBase{TBlock}"/> class.
        /// </summary>
        /// <param name="contentRepository">The content repository to inject.</param>
        protected BlockViewComponentBase(IContentRepository contentRepository)
        {
            ContentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        }

        /// <summary>
        /// Override to render the block model.
        /// </summary>
        /// <param name="model">The block model to render.</param>
        /// <returns>The view component result.</returns>
        public abstract Task<IViewComponentResult> InvokeAsync(TBlock model);
    }
}

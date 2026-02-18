using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    /// <summary>
    /// Fallback view component used when no custom ViewComponent is registered for a content type.
    /// Resolves views by convention at <c>Components/DefaultContent/{TypeName}</c>, stripping the
    /// "Element" suffix from the type name. Returns empty content if no matching view is found.
    /// </summary>
    public class DefaultContentViewComponent : ViewComponent
    {
        private readonly ICompositeViewEngine _viewEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContentViewComponent"/> class.
        /// </summary>
        /// <param name="viewEngine">The composite view engine for locating views by convention.</param>
        public DefaultContentViewComponent(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }

        /// <summary>
        /// Renders the given content model using a view resolved by convention.
        /// </summary>
        /// <param name="model">The content model to render.</param>
        /// <returns>A view result if a matching view is found; otherwise, empty content.</returns>
        public IViewComponentResult Invoke(IGraphContent model)
        {
            var typeName = model.GetType().Name;
            var viewName = typeName.EndsWith("Element") ? typeName[..^7] : typeName;

            var result = _viewEngine.FindView(ViewContext, $"Components/DefaultContent/{viewName}", false);
            if (result.Success)
            {
                return View(viewName, model);
            }

            return Content(string.Empty);
        }
    }
}

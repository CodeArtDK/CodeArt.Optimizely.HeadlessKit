using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    public class DefaultContentViewComponent : ViewComponent
    {
        private readonly ICompositeViewEngine _viewEngine;

        public DefaultContentViewComponent(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }

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

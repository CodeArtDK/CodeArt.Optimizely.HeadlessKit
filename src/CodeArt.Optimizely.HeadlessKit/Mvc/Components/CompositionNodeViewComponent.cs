using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.Core.Rendering;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    public class CompositionNodeViewComponent : ViewComponent
    {
        private readonly TemplateCoordinator _templateCoordinator;
        private readonly IDisplaySettingsResolver _displaySettingsResolver;
        private readonly IDisplayTemplateResolver? _displayTemplateResolver;

        public CompositionNodeViewComponent(
            TemplateCoordinator templateCoordinator,
            IDisplaySettingsResolver displaySettingsResolver,
            IDisplayTemplateResolver? displayTemplateResolver = null)
        {
            _templateCoordinator = templateCoordinator;
            _displaySettingsResolver = displaySettingsResolver;
            _displayTemplateResolver = displayTemplateResolver;
        }

        public async Task<IViewComponentResult> InvokeAsync(ICompositionNode node)
        {
            ViewBag.CssClasses = _displaySettingsResolver.ResolveCssClasses(node);
            ViewBag.DataAttributes = _displaySettingsResolver.ResolveDataAttributes(node);
            ViewBag.DisplayTemplate = _displayTemplateResolver?.Resolve(node.DisplayTemplateKey, node.DisplaySettings);

            if (node is CompositionComponentNode componentNode && componentNode.Component != null)
            {
                var componentType = _templateCoordinator.GetComponentForType(componentNode.Component.GetType());
                ViewBag.ComponentType = componentType;
                ViewBag.Component = componentNode.Component;
            }

            return View(node);
        }
    }
}

using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.Core.Rendering;
using CodeArt.Optimizely.HeadlessKit.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    public class CompositionRendererViewComponent : ViewComponent
    {
        private readonly IDisplaySettingsResolver _displaySettingsResolver;
        private readonly IDisplayTemplateResolver? _displayTemplateResolver;

        public CompositionRendererViewComponent(
            IDisplaySettingsResolver displaySettingsResolver,
            IDisplayTemplateResolver? displayTemplateResolver = null)
        {
            _displaySettingsResolver = displaySettingsResolver;
            _displayTemplateResolver = displayTemplateResolver;
        }

        public IViewComponentResult Invoke(ContentComposition? composition)
        {
            if (composition != null)
            {
                // Create a lightweight node adapter so the existing resolver can process root-level display settings
                var rootNode = new CompositionStructureNode
                {
                    NodeType = composition.NodeType,
                    DisplayTemplateKey = composition.DisplayTemplateKey,
                    DisplaySettings = composition.DisplaySettings
                };
                ViewBag.CssClasses = _displaySettingsResolver.ResolveCssClasses(rootNode);
                ViewBag.DataAttributes = _displaySettingsResolver.ResolveDataAttributes(rootNode);
                ViewBag.DisplayTemplate = _displayTemplateResolver?.Resolve(
                    composition.DisplayTemplateKey, composition.DisplaySettings);
                ViewBag.NodeType = composition.NodeType;
            }

            return View(composition);
        }
    }
}

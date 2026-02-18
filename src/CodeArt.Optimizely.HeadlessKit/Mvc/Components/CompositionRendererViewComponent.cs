using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.Core.Rendering;
using CodeArt.Optimizely.HeadlessKit.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    /// <summary>
    /// Top-level view component that renders an entire <see cref="ContentComposition"/> tree.
    /// Resolves CSS classes and data attributes from display settings and passes them via <c>ViewBag</c>.
    /// </summary>
    /// <remarks>
    /// Sets <c>ViewBag.CssClasses</c>, <c>ViewBag.DataAttributes</c>, <c>ViewBag.DisplayTemplate</c>,
    /// and <c>ViewBag.NodeType</c> for use by the associated view. Delegates rendering of child nodes
    /// to <see cref="CompositionNodeViewComponent"/>.
    /// </remarks>
    public class CompositionRendererViewComponent : ViewComponent
    {
        private readonly IDisplaySettingsResolver _displaySettingsResolver;
        private readonly IDisplayTemplateResolver? _displayTemplateResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionRendererViewComponent"/> class.
        /// </summary>
        /// <param name="displaySettingsResolver">Resolver for converting display settings to CSS classes and data attributes.</param>
        /// <param name="displayTemplateResolver">Optional resolver for display template names.</param>
        public CompositionRendererViewComponent(
            IDisplaySettingsResolver displaySettingsResolver,
            IDisplayTemplateResolver? displayTemplateResolver = null)
        {
            _displaySettingsResolver = displaySettingsResolver;
            _displayTemplateResolver = displayTemplateResolver;
        }

        /// <summary>
        /// Renders the specified composition tree.
        /// </summary>
        /// <param name="composition">The composition to render, or <c>null</c> to render an empty result.</param>
        /// <returns>The view component result containing the rendered composition.</returns>
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

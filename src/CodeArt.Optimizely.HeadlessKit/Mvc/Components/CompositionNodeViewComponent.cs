using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.Core.Rendering;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Components
{
    /// <summary>
    /// Renders individual composition nodes recursively. For structure nodes, renders a wrapper
    /// and recurses into children. For component nodes, resolves and invokes the appropriate
    /// ViewComponent via <see cref="TemplateCoordinator"/>.
    /// </summary>
    /// <remarks>
    /// Sets <c>ViewBag.CssClasses</c>, <c>ViewBag.DataAttributes</c>, <c>ViewBag.DisplayTemplate</c>,
    /// <c>ViewBag.ComponentType</c>, and <c>ViewBag.Component</c> for use by the associated view.
    /// </remarks>
    public class CompositionNodeViewComponent : ViewComponent
    {
        private readonly TemplateCoordinator _templateCoordinator;
        private readonly IDisplaySettingsResolver _displaySettingsResolver;
        private readonly IDisplayTemplateResolver? _displayTemplateResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionNodeViewComponent"/> class.
        /// </summary>
        /// <param name="templateCoordinator">Coordinator for resolving content type to ViewComponent mappings.</param>
        /// <param name="displaySettingsResolver">Resolver for converting display settings to CSS classes and data attributes.</param>
        /// <param name="displayTemplateResolver">Optional resolver for display template names.</param>
        public CompositionNodeViewComponent(
            TemplateCoordinator templateCoordinator,
            IDisplaySettingsResolver displaySettingsResolver,
            IDisplayTemplateResolver? displayTemplateResolver = null)
        {
            _templateCoordinator = templateCoordinator;
            _displaySettingsResolver = displaySettingsResolver;
            _displayTemplateResolver = displayTemplateResolver;
        }

        /// <summary>
        /// Renders the given composition node.
        /// </summary>
        /// <param name="node">The composition node to render.</param>
        /// <returns>The view component result containing the rendered node.</returns>
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

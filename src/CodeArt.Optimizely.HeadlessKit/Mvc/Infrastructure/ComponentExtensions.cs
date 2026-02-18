using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.Mvc.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    /// <summary>
    /// Extension methods on <see cref="IViewComponentHelper"/> for rendering content items
    /// via their registered ViewComponents.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Renders the registered ViewComponent for the given content item, falling back to
        /// <see cref="DefaultContentViewComponent"/> if no custom component is found.
        /// </summary>
        /// <param name="component">The view component helper.</param>
        /// <param name="graphContent">The content item to render.</param>
        /// <param name="renderingTag">Optional rendering tag to select an alternate template.</param>
        /// <returns>The rendered HTML content.</returns>
        public static async Task<IHtmlContent> InvokeGraphContentComponentAsync(this IViewComponentHelper component, IGraphContent graphContent, string? renderingTag = null)
        {
            // Resolve TemplateCoordinator from the ViewContext's HttpContext
            var viewContext = ExtractViewContext(component);
            var templateCoordinator = viewContext.HttpContext.RequestServices.GetRequiredService<TemplateCoordinator>();

            var viewcomponent = templateCoordinator.GetComponentForType(graphContent.GetType(), renderingTag);

            if (viewcomponent != null)
            {
                return await component.InvokeAsync(viewcomponent, graphContent);
            }

            return await component.InvokeAsync(typeof(DefaultContentViewComponent),
                new { model = graphContent });
        }

        /// <summary>
        /// Renders the registered ViewComponent for the content item within a composition node.
        /// Returns empty content if the node is not a component node or has no associated content.
        /// </summary>
        /// <param name="component">The view component helper.</param>
        /// <param name="node">The composition node containing the content to render.</param>
        /// <returns>The rendered HTML content, or <see cref="HtmlString.Empty"/> if the node has no component.</returns>
        public static async Task<IHtmlContent> InvokeGraphContentComponentAsync(this IViewComponentHelper component, ICompositionNode node)
        {
            if (node is not CompositionComponentNode componentNode || componentNode.Component == null)
                return HtmlString.Empty;

            return await component.InvokeGraphContentComponentAsync(componentNode.Component);
        }

        private static ViewContext ExtractViewContext(IViewComponentHelper component)
        {
            // DefaultViewComponentHelper stores ViewContext in a private field after Contextualize() is called.
            // Access it via reflection.
            var prop = component.GetType().GetProperty("ViewContext",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (prop?.GetValue(component) is ViewContext vc)
                return vc;

            // Fallback: check for a _viewContext field
            var field = component.GetType().GetField("_viewContext",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field?.GetValue(component) is ViewContext vc2)
                return vc2;

            throw new InvalidOperationException("Cannot resolve ViewContext from IViewComponentHelper. Ensure the helper has been contextualized.");
        }
    }
}

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
    public static class ComponentExtensions
    {
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

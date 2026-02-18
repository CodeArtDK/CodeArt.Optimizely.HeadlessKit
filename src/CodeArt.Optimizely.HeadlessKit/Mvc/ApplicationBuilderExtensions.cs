using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.Mvc.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArt.Optimizely.HeadlessKit.Mvc
{
    /// <summary>
    /// Extension methods on <see cref="IApplicationBuilder"/>, <see cref="IEndpointRouteBuilder"/>,
    /// and <see cref="WebApplication"/> for configuring CMS preview, content routing, and service initialization.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the CMS preview middleware that sets security headers allowing the Optimizely CMS editor
        /// to embed the site in an iframe for live preview. Should be called before <c>UseRouting()</c>.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder for chaining.</returns>
        public static IApplicationBuilder UseCmsPreview(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CmsPreviewMiddleware>();
        }

        /// <summary>
        /// Maps a catch-all dynamic route using <see cref="Infrastructure.ContentRouteTransformer"/>
        /// for MVC controller content routing. Use this in MVC sites (with controllers and views)
        /// instead of <see cref="MapContentPageRoute"/>.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <returns>The endpoint route builder for chaining.</returns>
        public static IEndpointRouteBuilder MapContentControllerRoute(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDynamicControllerRoute<ContentRouteTransformer>("{**path}");
            return endpoints;
        }

        /// <summary>
        /// Maps a catch-all dynamic route using <see cref="Infrastructure.ContentRouteTransformer"/>
        /// for Razor Pages content routing. Use this in Razor Pages sites
        /// instead of <see cref="MapContentControllerRoute"/>.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <returns>The endpoint route builder for chaining.</returns>
        public static IEndpointRouteBuilder MapContentPageRoute(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDynamicPageRoute<ContentRouteTransformer>("{**path}");
            return endpoints;
        }

        /// <summary>
        /// Initializes services that require async startup, such as <see cref="Rendering.TemplateCoordinator"/>
        /// endpoint scanning. Should be called after routing is configured.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <returns>A task representing the asynchronous initialization.</returns>
        public async static Task InitializeServicesAsync(this WebApplication app)
        {
            ServiceLocator.Initialize(app.Services.GetService<IServiceProviderProxy>());

            var services = app.Services.GetServices<IInitializable>();
            foreach (var service in services)
            {
                await service.InitializeAsync();
            }
        }
    }
}

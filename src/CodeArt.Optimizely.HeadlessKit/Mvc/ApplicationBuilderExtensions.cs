using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.Mvc.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArt.Optimizely.HeadlessKit.Mvc
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds middleware that allows the Optimizely CMS editor to embed the site
        /// in an iframe for live preview. Call before UseRouting().
        /// </summary>
        public static IApplicationBuilder UseCmsPreview(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CmsPreviewMiddleware>();
        }

        /// <summary>
        /// Maps the CMS catch-all route for MVC controllers.
        /// Use this in MVC sites (with controllers and views) instead of <see cref="MapContentPageRoute"/>.
        /// </summary>
        public static IEndpointRouteBuilder MapContentControllerRoute(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDynamicControllerRoute<ContentRouteTransformer>("{**path}");
            return endpoints;
        }

        /// <summary>
        /// Maps the CMS catch-all route for Razor Pages.
        /// Use this in Razor Pages sites instead of <see cref="MapContentControllerRoute"/>.
        /// </summary>
        public static IEndpointRouteBuilder MapContentPageRoute(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDynamicPageRoute<ContentRouteTransformer>("{**path}");
            return endpoints;
        }

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

using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.Mvc.Interfaces;
using CodeArt.Optimizely.HeadlessKit.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeArt.Optimizely.HeadlessKit.Mvc
{
    /// <summary>
    /// Extension methods for registering MVC integration services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the MVC integration services: <see cref="Infrastructure.ContentRouteTransformer"/>,
        /// <see cref="Rendering.TemplateCoordinator"/>, <see cref="Interfaces.IDisplaySettingsResolver"/>,
        /// and the content model binder.
        /// Usually called indirectly via <see cref="OptimizelyGraphServiceCollectionExtensions.AddOptimizelyGraph"/>.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddCodeArtOptimizelyGraphMvc(this IServiceCollection services)
        {
            services.AddSingleton<ContentRouteTransformer>();
            services.AddSingleton<TemplateCoordinator>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IServiceProviderProxy, HttpContextServiceProviderProxy>();

            services.AddSingleton<IInitializable, TemplateCoordinator>(sp => sp.GetService<TemplateCoordinator>()!);

            // Display settings resolver (replaceable by consumers)
            services.TryAddSingleton<IDisplaySettingsResolver, DefaultDisplaySettingsResolver>();

            services.Configure<MvcOptions>(options => options.ModelBinderProviders.Insert(0, new RoutedContentDataModelBinderProvider()));
            return services;
        }
    }
}

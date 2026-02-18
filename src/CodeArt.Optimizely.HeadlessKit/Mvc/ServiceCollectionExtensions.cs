using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.Mvc.Interfaces;
using CodeArt.Optimizely.HeadlessKit.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeArt.Optimizely.HeadlessKit.Mvc
{
    public static class ServiceCollectionExtensions
    {
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

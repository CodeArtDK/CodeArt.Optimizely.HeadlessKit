using CodeArt.Optimizely.HeadlessKit.Core;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.Mvc
{
    /// <summary>
    /// Extension methods for registering all Optimizely Graph services (content client and MVC integration).
    /// </summary>
    public static class OptimizelyGraphServiceCollectionExtensions
    {
        /// <summary>
        /// Registers both ContentClient and MVC services using settings from the "OptimizelyGraph" configuration section.
        /// This is the primary entry point for Graph services. It configures content type registry, query providers,
        /// <see cref="ContentClient.ContentGraphClient"/>, <see cref="Infrastructure.ContentRouteTransformer"/>,
        /// template coordination, display settings, and model binding.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The application configuration containing an "OptimizelyGraph" section.</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// builder.Services.AddOptimizelyGraph(builder.Configuration);
        /// </code>
        /// </example>
        public static IServiceCollection AddOptimizelyGraph(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind unified settings
            services.Configure<OptimizelyGraphSettings>(configuration.GetSection(OptimizelyGraphSettings.SectionName));

            // Bridge to ContentGraphOptions for backward compat
            services.AddSingleton<IConfigureOptions<ContentGraphOptions>>(sp =>
            {
                var unified = sp.GetRequiredService<IOptions<OptimizelyGraphSettings>>().Value;
                return new ConfigureNamedOptions<ContentGraphOptions>(Options.DefaultName, options =>
                {
                    options.Endpoint = unified.GraphEndpoint;
                    options.SingleKey = unified.SingleKey;
                    options.MainQuery = unified.MainQuery;
                    options.DebugLogging = unified.DebugLogging;
                    options.CmsAppUrl = unified.CmsAppUrl;
                });
            });

            // Register ContentClient services
            services.AddCodeArtOptimizelyGraphContentClient();

            // Register MVC services
            services.AddCodeArtOptimizelyGraphMvc();

            return services;
        }

        /// <summary>
        /// Registers only the MVC-specific Optimizely Graph services (routing, template coordination,
        /// model binding, display settings resolver, and tag helpers).
        /// Call this after <see cref="ContentClient.ServiceCollectionExtensions.AddCodeArtOptimizelyGraphContentClient"/>
        /// if you need to register MVC services separately.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddOptimizelyGraphMvc(this IServiceCollection services)
        {
            return services.AddCodeArtOptimizelyGraphMvc();
        }
    }
}

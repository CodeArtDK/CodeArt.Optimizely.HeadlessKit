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
    public static class OptimizelyGraphServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all Optimizely Graph services: content client, type registry,
        /// query provider, MVC routing, template coordination, and display settings.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration (reads "OptimizelyGraph" section).</param>
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
        /// Registers MVC-specific Optimizely Graph services: routing, template coordination,
        /// model binding, display settings resolver, and tag helpers.
        /// Call this after <see cref="ServiceCollectionExtensions.AddCodeArtOptimizelyGraphContentClient"/>.
        /// </summary>
        public static IServiceCollection AddOptimizelyGraphMvc(this IServiceCollection services)
        {
            return services.AddCodeArtOptimizelyGraphMvc();
        }
    }
}

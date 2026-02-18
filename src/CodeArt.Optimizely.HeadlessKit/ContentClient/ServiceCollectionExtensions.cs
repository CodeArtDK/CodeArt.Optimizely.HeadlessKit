using CodeArt.Optimizely.HeadlessKit.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Extension methods for registering ContentClient services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Optimizely Graph content client services: <see cref="ContentTypeRegistry"/>,
        /// <see cref="AutoGraphQueryProvider"/>, <see cref="ContentGraphClient"/>,
        /// <see cref="IContentRepository"/> (backed by <see cref="GraphContentRepository"/>),
        /// and <see cref="Microsoft.Extensions.Caching.Memory.IMemoryCache"/>.
        /// Usually called indirectly via <see cref="Mvc.OptimizelyGraphServiceCollectionExtensions.AddOptimizelyGraph"/>.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddCodeArtOptimizelyGraphContentClient(this IServiceCollection services)
        {
            services.AddSingleton<IContentTypeRegistry, ContentTypeRegistry>();
            services.AddSingleton<AutoGraphQueryProvider>();
            services.AddSingleton<IGraphQueryProvider, DefaultGraphQueryProvider>();
            services.AddSingleton<ContentGraphClient>();
            services.AddSingleton<IContentRepository, GraphContentRepository>();
            services.AddMemoryCache();

            return services;
        }
    }
}

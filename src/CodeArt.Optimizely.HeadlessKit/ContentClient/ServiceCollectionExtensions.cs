using CodeArt.Optimizely.HeadlessKit.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public static class ServiceCollectionExtensions
    {
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

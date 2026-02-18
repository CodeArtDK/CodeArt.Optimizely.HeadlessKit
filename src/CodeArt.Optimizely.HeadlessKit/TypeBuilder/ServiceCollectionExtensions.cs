using CodeArt.Optimizely.HeadlessKit.Core.Rendering;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Clients;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using IdentityModel.Client;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder
{
    /// <summary>
    /// Extension methods for registering SaaS CMS TypeBuilder services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all TypeBuilder services using settings from the "SaaSCMS" configuration section.
        /// This configures OAuth2 token management, <see cref="SaaSCMSClient"/>, <see cref="AppDomainScanner"/>,
        /// <see cref="CMSTypeSyncService"/>, the hosted sync service, and <see cref="IDisplayTemplateResolver"/>.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The application configuration containing a "SaaSCMS" section.</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
        /// </code>
        /// </example>
        public static IServiceCollection AddSaaSCMSTypeBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SaaSCMSSettings>(configuration.GetSection(SaaSCMSSettings.SectionName));
            return services.AddSaaSCMSTypeBuilderCore();
        }

        /// <summary>
        /// Registers all TypeBuilder services with inline settings configuration via a delegate.
        /// This configures OAuth2 token management, <see cref="SaaSCMSClient"/>, <see cref="AppDomainScanner"/>,
        /// <see cref="CMSTypeSyncService"/>, the hosted sync service, and <see cref="IDisplayTemplateResolver"/>.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configureSettings">A delegate to configure <see cref="SaaSCMSSettings"/>.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddSaaSCMSTypeBuilder(this IServiceCollection services, Action<SaaSCMSSettings> configureSettings)
        {
            services.Configure(configureSettings);
            return services.AddSaaSCMSTypeBuilderCore();
        }

        /// <summary>
        /// Registers all TypeBuilder services expecting <see cref="IOptions{SaaSCMSSettings}"/> to be already
        /// configured in the container. Use this overload when you bind settings yourself before calling this method.
        /// This configures OAuth2 token management, <see cref="SaaSCMSClient"/>, <see cref="AppDomainScanner"/>,
        /// <see cref="CMSTypeSyncService"/>, the hosted sync service, and <see cref="IDisplayTemplateResolver"/>.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddSaaSCMSTypeBuilder(this IServiceCollection services)
        {
            services.AddOptions<SaaSCMSSettings>();
            return services.AddSaaSCMSTypeBuilderCore();
        }

        private static IServiceCollection AddSaaSCMSTypeBuilderCore(this IServiceCollection services)
        {
            services.AddAccessTokenManagement((provider, options) =>
            {
                var cmsSettings = provider.GetRequiredService<IOptions<SaaSCMSSettings>>().Value;
                var tokenEndpoint = new Uri(cmsSettings.TokenEndpoint, UriKind.RelativeOrAbsolute);
                var requestUri = tokenEndpoint.IsAbsoluteUri
                    ? tokenEndpoint
                    : new Uri(new Uri(cmsSettings.ApiBaseUrl), tokenEndpoint);

                options.Client.Clients[SaaSCMSSettings.DefaultTokenClientName] = new ClientCredentialsTokenRequest
                {
                    RequestUri = requestUri,
                    ClientId = cmsSettings.ClientId,
                    ClientSecret = cmsSettings.ClientSecret,
                    Scope = cmsSettings.Scope,
                    GrantType = "client_credentials"
                };
            });

            services.AddHttpClient<SaaSCMSClient>((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<SaaSCMSSettings>>().Value;
                client.BaseAddress = new Uri(settings.ApiBaseUrl);
                client.Timeout = settings.HttpTimeout;
            }).AddClientAccessTokenHandler(SaaSCMSSettings.DefaultTokenClientName);

            services.AddSingleton<AppDomainScanner>();
            services.AddSingleton<IContentTypeProvider>(provider => provider.GetRequiredService<AppDomainScanner>());
            services.AddSingleton<IDisplayTemplateProvider>(provider => provider.GetRequiredService<AppDomainScanner>());
            services.AddSingleton<CMSTypeSyncService>();
            services.AddHostedService<SaaSCMSTypeSyncHostedService>();
            services.AddSingleton<IDisplayTemplateResolver, DisplayTemplateResolver>();

            return services;
        }
    }
}

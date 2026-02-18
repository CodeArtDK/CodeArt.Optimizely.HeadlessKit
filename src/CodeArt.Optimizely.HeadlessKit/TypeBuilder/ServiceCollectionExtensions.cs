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
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSaaSCMSTypeBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SaaSCMSSettings>(configuration.GetSection(SaaSCMSSettings.SectionName));
            return services.AddSaaSCMSTypeBuilderCore();
        }

        public static IServiceCollection AddSaaSCMSTypeBuilder(this IServiceCollection services, Action<SaaSCMSSettings> configureSettings)
        {
            services.Configure(configureSettings);
            return services.AddSaaSCMSTypeBuilderCore();
        }

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

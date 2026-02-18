using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder
{
    public class SaaSCMSTypeSyncHostedService : BackgroundService
    {
        private readonly CMSTypeSyncService _syncService;
        private readonly SaaSCMSSettings _settings;
        private readonly ILogger<SaaSCMSTypeSyncHostedService> _logger;

        public SaaSCMSTypeSyncHostedService(
            CMSTypeSyncService syncService,
            IOptions<SaaSCMSSettings> settings,
            ILogger<SaaSCMSTypeSyncHostedService> logger)
        {
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_settings.SyncOnStartup)
            {
                _logger.LogInformation("SaaS CMS sync on startup is disabled.");
                return;
            }

            await _syncService.SyncContentTypes(stoppingToken);
            await _syncService.SyncDisplayTemplates(stoppingToken);
        }
    }
}

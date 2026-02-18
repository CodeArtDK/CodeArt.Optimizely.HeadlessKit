using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Clients;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Sync;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder
{
    /// <summary>
    /// Orchestrates synchronization of locally-defined content types and display templates
    /// with the remote Optimizely SaaS CMS API. Creates new types and updates changed types,
    /// but never deletes to avoid accidental content loss.
    /// </summary>
    public class CMSTypeSyncService
    {
        private readonly SaaSCMSClient _cmsClient;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IDisplayTemplateProvider _displayTemplateProvider;
        private readonly SaaSCMSSettings _settings;
        private readonly ILogger<CMSTypeSyncService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CMSTypeSyncService"/> class.
        /// </summary>
        /// <param name="saaSCMSClient">The CMS REST API client.</param>
        /// <param name="contentTypeProvider">Provider for locally-discovered content type definitions.</param>
        /// <param name="displayTemplateProvider">Provider for locally-discovered display template definitions.</param>
        /// <param name="settings">The SaaS CMS settings.</param>
        /// <param name="logger">Logger instance.</param>
        public CMSTypeSyncService(
            SaaSCMSClient saaSCMSClient,
            IContentTypeProvider contentTypeProvider,
            IDisplayTemplateProvider displayTemplateProvider,
            IOptions<SaaSCMSSettings> settings,
            ILogger<CMSTypeSyncService> logger)
        {
            _cmsClient = saaSCMSClient ?? throw new ArgumentNullException(nameof(saaSCMSClient));
            _contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
            _displayTemplateProvider = displayTemplateProvider ?? throw new ArgumentNullException(nameof(displayTemplateProvider));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Discovers content types from annotations, compares them with remote definitions,
        /// and creates or updates as needed. Returns a report of the sync results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="SyncReport"/> describing created, updated, unchanged, and failed types.</returns>
        public async Task<SyncReport> SyncContentTypes(CancellationToken cancellationToken = default)
        {
            var report = new SyncReport();

            if (_contentTypeProvider.ContentTypes.Count == 0)
            {
                _logger.LogInformation("No SaaS CMS content types discovered for syncing.");
                return report;
            }

            var existing = await _cmsClient.ListContentTypes(cancellationToken);
            var existingByKey = new Dictionary<string, SaaSContentType>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in existing.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Key))
                    existingByKey[item.Key] = item;
            }

            // Sort types so that those referenced by MayContainTypes are synced first.
            var ordered = TopologicalSort(_contentTypeProvider.ContentTypes);

            foreach (var type in ordered)
            {
                if (string.IsNullOrWhiteSpace(type.Key))
                {
                    _logger.LogWarning("Skipping content type without a key.");
                    continue;
                }

                try
                {
                    if (existingByKey.TryGetValue(type.Key, out var remote) && _settings.UpdateExistingContentTypes)
                    {
                        var comparer = new ContentTypeComparer(type, remote);
                        if (comparer.HasChanges)
                        {
                            var patch = comparer.BuildPatchPayload();
                            await _cmsClient.UpdateContentType(type.Key, patch, _settings.IgnoreDataLossWarnings, cancellationToken);
                            report.Updated.Add(type.Key);
                            _logger.LogInformation("Updated content type '{Key}'.", type.Key);
                        }
                        else
                        {
                            report.Unchanged.Add(type.Key);
                        }
                    }
                    else if (!existingByKey.ContainsKey(type.Key))
                    {
                        await _cmsClient.CreateContentType(type, cancellationToken);
                        report.Created.Add(type.Key);
                        _logger.LogInformation("Created content type '{Key}'.", type.Key);
                    }
                    else
                    {
                        report.Unchanged.Add(type.Key);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Failed to sync content type '{Key}'.", type.Key);
                    report.Failed.Add(new SyncFailure(type.Key, ex.Message));
                }
            }

            _logger.LogInformation("Content type sync complete. {Report}", report);
            return report;
        }

        /// <summary>
        /// Discovers display templates from annotations, compares them with remote definitions,
        /// and creates or updates as needed. Returns a report of the sync results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="SyncReport"/> describing created, updated, unchanged, and failed templates.</returns>
        public async Task<SyncReport> SyncDisplayTemplates(CancellationToken cancellationToken = default)
        {
            var report = new SyncReport();

            if (_displayTemplateProvider.DisplayTemplates.Count == 0)
            {
                _logger.LogInformation("No SaaS CMS display templates discovered for syncing.");
                return report;
            }

            var existing = await _cmsClient.ListDisplayTemplates(cancellationToken);
            var existingByKey = new Dictionary<string, SaaSDisplayTemplate>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in existing.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Key))
                    existingByKey[item.Key] = item;
            }

            foreach (var template in _displayTemplateProvider.DisplayTemplates)
            {
                if (string.IsNullOrWhiteSpace(template.Key))
                {
                    _logger.LogWarning("Skipping display template without a key.");
                    continue;
                }

                try
                {
                    if (existingByKey.TryGetValue(template.Key, out var remote) && _settings.UpdateExistingDisplayTemplates)
                    {
                        var comparer = new DisplayTemplateComparer(template, remote);
                        if (comparer.HasChanges)
                        {
                            var patch = comparer.BuildPatchPayload();
                            await _cmsClient.UpdateDisplayTemplate(template.Key, patch, cancellationToken);
                            report.Updated.Add(template.Key);
                            _logger.LogInformation("Updated display template '{Key}'.", template.Key);
                        }
                        else
                        {
                            report.Unchanged.Add(template.Key);
                        }
                    }
                    else if (!existingByKey.ContainsKey(template.Key))
                    {
                        await _cmsClient.CreateDisplayTemplate(template, cancellationToken);
                        report.Created.Add(template.Key);
                        _logger.LogInformation("Created display template '{Key}'.", template.Key);
                    }
                    else
                    {
                        report.Unchanged.Add(template.Key);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Failed to sync display template '{Key}'.", template.Key);
                    report.Failed.Add(new SyncFailure(template.Key, ex.Message));
                }
            }

            _logger.LogInformation("Display template sync complete. {Report}", report);
            return report;
        }

        /// <summary>
        /// Sorts content types so that types referenced by another type's
        /// <see cref="SaaSContentType.MayContainTypes"/> are ordered before the
        /// types that reference them. This ensures referenced types exist in the
        /// API before the referencing type is created.
        /// </summary>
        internal static IReadOnlyList<SaaSContentType> TopologicalSort(IReadOnlyCollection<SaaSContentType> types)
        {
            var byKey = new Dictionary<string, SaaSContentType>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in types)
            {
                if (!string.IsNullOrWhiteSpace(t.Key))
                    byKey[t.Key] = t;
            }

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<SaaSContentType>(types.Count);

            void Visit(SaaSContentType type)
            {
                if (string.IsNullOrWhiteSpace(type.Key) || !visited.Add(type.Key))
                    return;

                // Visit dependencies (types referenced by MayContainTypes) first
                if (type.MayContainTypes != null)
                {
                    foreach (var dep in type.MayContainTypes)
                    {
                        if (byKey.TryGetValue(dep, out var depType))
                            Visit(depType);
                    }
                }

                result.Add(type);
            }

            foreach (var type in types)
                Visit(type);

            return result;
        }
    }
}

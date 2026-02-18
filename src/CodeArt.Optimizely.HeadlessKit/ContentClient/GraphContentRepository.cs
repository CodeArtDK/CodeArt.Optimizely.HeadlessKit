using CodeArt.Optimizely.HeadlessKit.Core;
using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// <see cref="IContentRepository"/> implementation with <see cref="IMemoryCache"/>-based caching.
    /// Cache duration is controlled by <see cref="ContentGraphOptions.CacheDurationSeconds"/> (default 300 seconds).
    /// Set <see cref="ContentGraphOptions.CacheDurationSeconds"/> to 0 to disable caching.
    /// </summary>
    public class GraphContentRepository(ContentGraphClient client, IMemoryCache memoryCache, IOptions<ContentGraphOptions> options) : IContentRepository
    {
        private readonly ContentGraphClient _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly IMemoryCache _memCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        private readonly ContentGraphOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        /// <inheritdoc />
        public async Task<CT?> GetContent<CT>(string key, string[]? locale = null) where CT : class, IGraphContent
        {
            if (_options.CacheDurationSeconds <= 0)
                return await _client.GetContentByKey<CT>(key, locale);

            return await _memCache.GetOrCreateAsync("content_" + key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.CacheDurationSeconds);
                return _client.GetContentByKey<CT>(key, locale);
            });
        }

        /// <inheritdoc />
        public async Task<CT?> GetContentByPath<CT>(string path, string[]? locale = null) where CT : class, IGraphContent
        {
            if (_options.CacheDurationSeconds <= 0)
                return await _client.GetContentByPath<CT>(path, locale);

            return await _memCache.GetOrCreateAsync("content_path_" + path, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.CacheDurationSeconds);
                return _client.GetContentByPath<CT>(path, locale);
            });
        }

        /// <inheritdoc />
        public async Task<List<CT>?> GetChildren<CT>(string parentkey, string[]? locale = null) where CT : class, IGraphContent
        {
            if (_options.CacheDurationSeconds <= 0)
                return await _client.GetChildren<CT>(parentkey, locale);

            return await _memCache.GetOrCreateAsync("children_" + parentkey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.CacheDurationSeconds);
                return _client.GetChildren<CT>(parentkey, locale);
            });
        }
    }
}

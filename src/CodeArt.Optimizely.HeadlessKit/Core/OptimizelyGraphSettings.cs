namespace CodeArt.Optimizely.HeadlessKit.Core
{
    /// <summary>
    /// Unified configuration settings for Optimizely Graph, bound from the "OptimizelyGraph" appsettings section.
    /// Combines CMS API settings (for type sync) and Graph query settings (for content retrieval).
    /// </summary>
    public class OptimizelyGraphSettings
    {
        /// <summary>
        /// The appsettings configuration section name.
        /// </summary>
        public const string SectionName = "OptimizelyGraph";

        // === CMS API settings (from SaaSCMSSettings) ===

        /// <summary>
        /// Gets or sets the CMS REST API base URL.
        /// </summary>
        public string ApiBaseUrl { get; set; } = "https://api.cms.optimizely.com/";

        /// <summary>
        /// Gets or sets the API version path prefix.
        /// </summary>
        public string ApiPathPrefix { get; set; } = "preview3";

        /// <summary>
        /// Gets or sets the OAuth2 token endpoint URL path.
        /// </summary>
        public string TokenEndpoint { get; set; } = "/oauth/token";

        /// <summary>
        /// Gets or sets the OAuth2 client ID. Required.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the OAuth2 client secret. Required.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the OAuth2 scope.
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// Gets or sets a filter for content type sources to sync.
        /// </summary>
        public string? ContentTypeSources { get; set; }

        /// <summary>
        /// Gets or sets whether to synchronize content types at application startup. Default is true.
        /// </summary>
        public bool SyncOnStartup { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to update existing remote content types when local definitions change. Default is true.
        /// </summary>
        public bool UpdateExistingContentTypes { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to update existing remote display templates when local definitions change. Default is true.
        /// </summary>
        public bool UpdateExistingDisplayTemplates { get; set; } = true;

        /// <summary>
        /// Gets or sets the HTTP client timeout for CMS API requests.
        /// </summary>
        public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromMinutes(5);

        // === Graph query settings (from ContentGraphOptions) ===

        /// <summary>
        /// Gets or sets the Optimizely Graph API endpoint URL.
        /// </summary>
        public string GraphEndpoint { get; set; } = "https://cg.optimizely.com/content/v2";

        /// <summary>
        /// Gets or sets the single-key authentication token for the Graph API.
        /// </summary>
        public string SingleKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional custom main GraphQL query. When null, the query is auto-generated from registered content types.
        /// </summary>
        public string? MainQuery { get; set; }

        /// <summary>
        /// When true, GraphQL queries and responses are written to the console.
        /// </summary>
        public bool DebugLogging { get; set; }

        /// <summary>
        /// The CMS application URL (e.g., https://app-xxxx.cms.optimizely.com).
        /// Required for live preview support. Leave empty to disable preview script injection.
        /// </summary>
        public string? CmsAppUrl { get; set; }
    }
}

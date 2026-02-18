namespace CodeArt.Optimizely.HeadlessKit.Core
{
    public class OptimizelyGraphSettings
    {
        public const string SectionName = "OptimizelyGraph";

        // === CMS API settings (from SaaSCMSSettings) ===
        public string ApiBaseUrl { get; set; } = "https://api.cms.optimizely.com/";
        public string ApiPathPrefix { get; set; } = "preview3";
        public string TokenEndpoint { get; set; } = "/oauth/token";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string? Scope { get; set; }
        public string? ContentTypeSources { get; set; }
        public bool SyncOnStartup { get; set; } = true;
        public bool UpdateExistingContentTypes { get; set; } = true;
        public bool UpdateExistingDisplayTemplates { get; set; } = true;
        public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromMinutes(5);

        // === Graph query settings (from ContentGraphOptions) ===
        public string GraphEndpoint { get; set; } = "https://cg.optimizely.com/content/v2";
        public string SingleKey { get; set; } = string.Empty;
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

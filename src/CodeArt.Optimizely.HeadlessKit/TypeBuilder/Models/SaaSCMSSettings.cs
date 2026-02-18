using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Configuration settings for the SaaS CMS TypeBuilder, bound from the "SaaSCMS" appsettings section.
    /// </summary>
    public class SaaSCMSSettings
    {
        /// <summary>
        /// The appsettings configuration section name.
        /// </summary>
        public const string SectionName = "SaaSCMS";

        /// <summary>
        /// The default named HTTP client name used for token management.
        /// </summary>
        public const string DefaultTokenClientName = "SaaSAPI";

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
        /// Gets or sets a filter for content type sources to include during sync.
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
        /// Gets or sets whether to ignore API data loss warnings during content type updates. Default is false.
        /// </summary>
        public bool IgnoreDataLossWarnings { get; set; } = false;

        /// <summary>
        /// Gets or sets the HTTP client timeout for CMS API requests.
        /// </summary>
        public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromMinutes(5);
    }
}

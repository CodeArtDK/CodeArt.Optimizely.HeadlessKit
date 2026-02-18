using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core
{
    /// <summary>
    /// Configuration options for Optimizely Graph content queries.
    /// </summary>
    public class ContentGraphOptions
    {
        /// <summary>
        /// Gets or sets the Optimizely Graph API endpoint URL.
        /// </summary>
        public string Endpoint { get; set; } = "https://cg.optimizely.com/content/v2";

        /// <summary>
        /// Gets or sets the single-key authentication token for the Graph API.
        /// </summary>
        public string SingleKey { get; set; }

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

        /// <summary>
        /// Cache duration in seconds for content queries in <see cref="ContentClient"/>.
        /// Set to 0 to disable caching. Default is 300 (5 minutes).
        /// </summary>
        public int CacheDurationSeconds { get; set; } = 300;
    }
}

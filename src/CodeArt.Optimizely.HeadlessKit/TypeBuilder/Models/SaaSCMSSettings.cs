using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    public class SaaSCMSSettings
    {
        public const string SectionName = "SaaSCMS";
        public const string DefaultTokenClientName = "SaaSAPI";

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

        public bool IgnoreDataLossWarnings { get; set; } = false;

        public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromMinutes(5);
    }
}

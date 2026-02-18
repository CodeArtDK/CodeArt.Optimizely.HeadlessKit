using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Feature flags that can be enabled on content types.
    /// </summary>
    public enum Features
    {
        /// <summary>
        /// Enables multi-language support for the content type.
        /// </summary>
        Localization,

        /// <summary>
        /// Enables content version history.
        /// </summary>
        Versioning,

        /// <summary>
        /// Enables scheduled publish and unpublish periods.
        /// </summary>
        PublishPeriod,

        /// <summary>
        /// Enables URL routing for page content types.
        /// </summary>
        Routing,

        /// <summary>
        /// Enables binary file storage for media content types.
        /// </summary>
        Binary
    }
}

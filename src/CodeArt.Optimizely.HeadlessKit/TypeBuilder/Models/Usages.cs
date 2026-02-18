using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Defines how a content type can be used in the CMS.
    /// </summary>
    public enum Usages
    {
        /// <summary>
        /// The content type can be embedded as a property in other types.
        /// </summary>
        Property,

        /// <summary>
        /// The content type can exist as a standalone content instance.
        /// </summary>
        Instance
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Defines the base content types available in Optimizely CMS SaaS.
    /// </summary>
    public enum BaseTypes
    {
        /// <summary>
        /// A routable page with its own URL.
        /// </summary>
        Page,

        /// <summary>
        /// A reusable content block that can be embedded in other content.
        /// </summary>
        Block,

        /// <summary>
        /// Base type for media assets.
        /// </summary>
        Media,

        /// <summary>
        /// An image media asset.
        /// </summary>
        Image,

        /// <summary>
        /// A video media asset.
        /// </summary>
        Video,

        /// <summary>
        /// A container for organizing content items.
        /// </summary>
        Folder,

        /// <summary>
        /// A Visual Builder page with composition support.
        /// </summary>
        Experience,

        /// <summary>
        /// A Visual Builder layout section.
        /// </summary>
        Section,

        /// <summary>
        /// A Visual Builder component or element.
        /// </summary>
        Element
    }
}

using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Base class for Visual Builder experience pages. Includes a composition tree
    /// that defines the page layout and its embedded components.
    /// </summary>
    public class GraphExperience : GraphPageContent
    {
        /// <summary>
        /// Gets or sets the composition tree defining the page layout and components.
        /// </summary>
        public ContentComposition? Composition { get; set; }
    }
}

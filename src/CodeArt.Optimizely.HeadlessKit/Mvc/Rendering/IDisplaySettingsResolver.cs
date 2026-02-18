using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Rendering
{
    /// <summary>
    /// Converts composition display settings into CSS classes and HTML data attributes for rendering.
    /// Register a custom implementation in DI to override the default behavior.
    /// </summary>
    public interface IDisplaySettingsResolver
    {
        /// <summary>
        /// Returns a space-separated string of CSS classes derived from the node's display settings.
        /// </summary>
        /// <param name="node">The composition node to resolve CSS classes for.</param>
        /// <returns>A space-separated string of CSS classes, or an empty string if none apply.</returns>
        string ResolveCssClasses(ICompositionNode node);

        /// <summary>
        /// Returns a dictionary of HTML data attributes derived from the node's display settings.
        /// </summary>
        /// <param name="node">The composition node to resolve data attributes for.</param>
        /// <returns>A dictionary mapping attribute names to values.</returns>
        IDictionary<string, string> ResolveDataAttributes(ICompositionNode node);
    }
}

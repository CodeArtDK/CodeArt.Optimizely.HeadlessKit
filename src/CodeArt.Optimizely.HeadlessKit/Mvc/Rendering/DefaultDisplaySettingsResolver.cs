using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Rendering
{
    /// <summary>
    /// Default <see cref="IDisplaySettingsResolver"/> that generates <c>opti-{value}</c> CSS classes
    /// and <c>data-display-{key}</c> HTML data attributes from composition display settings.
    /// Replace via DI with a custom implementation for different naming conventions.
    /// </summary>
    public class DefaultDisplaySettingsResolver : IDisplaySettingsResolver
    {
        /// <inheritdoc />
        public string ResolveCssClasses(ICompositionNode node)
        {
            if (node.DisplaySettings == null || node.DisplaySettings.Count == 0)
                return string.Empty;

            var classes = node.DisplaySettings
                .Where(ds => !string.IsNullOrWhiteSpace(ds.Value))
                .Select(ds => $"opti-{ds.Value}")
                .ToList();

            return string.Join(" ", classes);
        }

        /// <inheritdoc />
        public IDictionary<string, string> ResolveDataAttributes(ICompositionNode node)
        {
            var attributes = new Dictionary<string, string>();

            if (node.DisplaySettings == null)
                return attributes;

            foreach (var setting in node.DisplaySettings)
            {
                if (!string.IsNullOrWhiteSpace(setting.Key) && setting.Value != null)
                {
                    attributes[$"display-{setting.Key}"] = setting.Value;
                }
            }

            return attributes;
        }
    }
}

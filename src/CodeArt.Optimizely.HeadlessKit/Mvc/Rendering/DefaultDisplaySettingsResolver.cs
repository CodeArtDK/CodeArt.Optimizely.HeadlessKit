using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Rendering
{
    public class DefaultDisplaySettingsResolver : IDisplaySettingsResolver
    {
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

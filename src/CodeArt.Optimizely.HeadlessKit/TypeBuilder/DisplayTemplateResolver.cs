using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.Core.Rendering;
using System.Reflection;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder
{
    public class DisplayTemplateResolver : IDisplayTemplateResolver
    {
        private readonly Lazy<Dictionary<string, Type>> _templateTypes;

        public DisplayTemplateResolver(IDisplayTemplateProvider provider)
        {
            _templateTypes = new Lazy<Dictionary<string, Type>>(() =>
                provider.DisplayTemplates
                    .Where(t => !string.IsNullOrWhiteSpace(t.Key))
                    .ToDictionary(t => t.Key!, t => t.GetType()));
        }

        public object? Resolve(string? displayTemplateKey, List<CompositionDisplaySetting>? displaySettings)
        {
            if (string.IsNullOrWhiteSpace(displayTemplateKey))
                return null;

            if (!_templateTypes.Value.TryGetValue(displayTemplateKey, out var templateType))
                return null;

            var instance = Activator.CreateInstance(templateType);
            if (instance == null || displaySettings == null)
                return instance;

            foreach (var setting in displaySettings)
            {
                if (string.IsNullOrWhiteSpace(setting.Key))
                    continue;

                var prop = templateType.GetProperty(setting.Key, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite && setting.Value != null)
                {
                    prop.SetValue(instance, setting.Value);
                }
            }

            return instance;
        }
    }
}

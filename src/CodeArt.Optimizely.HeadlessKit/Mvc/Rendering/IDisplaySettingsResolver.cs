using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Rendering
{
    public interface IDisplaySettingsResolver
    {
        string ResolveCssClasses(ICompositionNode node);
        IDictionary<string, string> ResolveDataAttributes(ICompositionNode node);
    }
}

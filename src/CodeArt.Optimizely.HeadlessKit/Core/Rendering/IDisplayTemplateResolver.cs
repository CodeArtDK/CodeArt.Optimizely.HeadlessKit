using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;

namespace CodeArt.Optimizely.HeadlessKit.Core.Rendering
{
    /// <summary>
    /// Resolves a strongly-typed display template instance from a display template key
    /// and the per-instance display settings returned by Graph.
    /// </summary>
    public interface IDisplayTemplateResolver
    {
        object? Resolve(string? displayTemplateKey, List<CompositionDisplaySetting>? displaySettings);
    }
}

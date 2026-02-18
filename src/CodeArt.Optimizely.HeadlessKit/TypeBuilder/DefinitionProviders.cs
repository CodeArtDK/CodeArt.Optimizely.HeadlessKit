using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.Collections.Generic;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder
{
    public interface IContentTypeProvider
    {
        IReadOnlyCollection<SaaSContentType> ContentTypes { get; }
    }

    public interface IDisplayTemplateProvider
    {
        IReadOnlyCollection<SaaSDisplayTemplate> DisplayTemplates { get; }
    }
}

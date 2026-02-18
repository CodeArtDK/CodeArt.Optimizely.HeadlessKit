using CodeArt.Optimizely.HeadlessKit.Core.Models;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public interface IContentTypeRegistry
    {
        IReadOnlyCollection<Type> PageTypes { get; }
        IReadOnlyCollection<Type> ComponentTypes { get; }
        IReadOnlyCollection<Type> AllTypes { get; }
        Type? ResolveType(string typeName);
    }
}

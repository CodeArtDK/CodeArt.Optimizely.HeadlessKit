using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Reflection;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public class ContentTypeRegistry : IContentTypeRegistry
    {
        private readonly List<Type> _pageTypes = new();
        private readonly List<Type> _componentTypes = new();
        private readonly List<Type> _allTypes = new();
        private readonly Dictionary<string, Type> _typeMap = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyCollection<Type> PageTypes => _pageTypes;
        public IReadOnlyCollection<Type> ComponentTypes => _componentTypes;
        public IReadOnlyCollection<Type> AllTypes => _allTypes;

        public ContentTypeRegistry()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsAbstract)
                            continue;

                        if (typeof(IGraphContent).IsAssignableFrom(type))
                        {
                            _allTypes.Add(type);
                            _typeMap[type.Name] = type;

                            if (typeof(IGraphPageContent).IsAssignableFrom(type))
                                _pageTypes.Add(type);
                            else if (typeof(IGraphComponentContent).IsAssignableFrom(type))
                                _componentTypes.Add(type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // Some assemblies may not be loadable
                }
            }
        }

        public Type? ResolveType(string typeName)
        {
            return _typeMap.GetValueOrDefault(typeName);
        }
    }
}

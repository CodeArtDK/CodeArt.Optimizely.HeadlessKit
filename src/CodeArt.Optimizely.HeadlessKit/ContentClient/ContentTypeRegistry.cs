using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Reflection;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Default <see cref="IContentTypeRegistry"/> implementation that scans all loaded
    /// <see cref="AppDomain"/> assemblies for concrete <see cref="IGraphContent"/> implementations at construction time.
    /// </summary>
    public class ContentTypeRegistry : IContentTypeRegistry
    {
        private readonly List<Type> _pageTypes = new();
        private readonly List<Type> _componentTypes = new();
        private readonly List<Type> _allTypes = new();
        private readonly Dictionary<string, Type> _typeMap = new(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        public IReadOnlyCollection<Type> PageTypes => _pageTypes;

        /// <inheritdoc />
        public IReadOnlyCollection<Type> ComponentTypes => _componentTypes;

        /// <inheritdoc />
        public IReadOnlyCollection<Type> AllTypes => _allTypes;

        /// <summary>
        /// Initializes a new instance of <see cref="ContentTypeRegistry"/> by scanning all loaded assemblies.
        /// </summary>
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

        /// <inheritdoc />
        public Type? ResolveType(string typeName)
        {
            return _typeMap.GetValueOrDefault(typeName);
        }
    }
}

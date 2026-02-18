using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Custom converter for IGraphContent that resolves __typename at any position in the
    /// JSON object. System.Text.Json's built-in polymorphism requires the discriminator to
    /// be the first property, but Optimizely Graph's _json responses don't guarantee this.
    /// </summary>
    public class GraphContentJsonConverter : JsonConverter<IGraphContent>
    {
        private static readonly Lazy<Dictionary<string, Type>> _typeMap = new(BuildTypeMap);

        public override bool CanConvert(Type typeToConvert)
        {
            // Only handle IGraphContent interface directly, not specific implementations.
            // This prevents infinite recursion when deserializing resolved concrete types.
            return typeToConvert == typeof(IGraphContent);
        }

        public override IGraphContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var typeName = root.TryGetProperty("__typename", out var tn) && tn.ValueKind == JsonValueKind.String
                ? tn.GetString()
                : null;

            Type targetType = typeof(GraphContent);
            if (typeName != null && _typeMap.Value.TryGetValue(typeName, out var resolved))
                targetType = resolved;

            return (IGraphContent?)root.Deserialize(targetType, options);
        }

        public override void Write(Utf8JsonWriter writer, IGraphContent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }

        /// <summary>
        /// Resolves a __typename string to a .NET Type, or null if not found.
        /// </summary>
        public static Type? ResolveType(string typeName)
        {
            return _typeMap.Value.TryGetValue(typeName, out var type) ? type : null;
        }

        private static Dictionary<string, Type> BuildTypeMap()
        {
            var map = new Dictionary<string, Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract && typeof(IGraphContent).IsAssignableFrom(type))
                        {
                            map[type.Name] = type;
                        }
                    }
                }
                catch (ReflectionTypeLoadException) { }
            }
            return map;
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models.Composition
{
    /// <summary>
    /// Custom converter for ICompositionNode that distinguishes structure nodes from
    /// component nodes. Uses __typename when present, falls back to nodeType heuristic.
    /// </summary>
    public class CompositionNodeJsonConverter : JsonConverter<ICompositionNode>
    {
        public override ICompositionNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // Check __typename first (present in _json responses)
            if (root.TryGetProperty("__typename", out var typeNameEl) && typeNameEl.ValueKind == JsonValueKind.String)
            {
                var typeName = typeNameEl.GetString();
                if (typeName == "CompositionComponentNode")
                    return root.Deserialize<CompositionComponentNode>(options);
                if (typeName == "CompositionStructureNode")
                    return root.Deserialize<CompositionStructureNode>(options);
            }

            // Fallback: use nodeType to distinguish
            if (root.TryGetProperty("nodeType", out var nodeTypeEl) && nodeTypeEl.ValueKind == JsonValueKind.String)
            {
                var nodeType = nodeTypeEl.GetString();
                if (nodeType == "component")
                    return root.Deserialize<CompositionComponentNode>(options);
            }

            // Default to structure node
            return root.Deserialize<CompositionStructureNode>(options);
        }

        public override void Write(Utf8JsonWriter writer, ICompositionNode value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public static class GraphFieldMapper
    {
        private static readonly HashSet<Type> _scalarTypes = new()
        {
            typeof(string), typeof(int), typeof(int?),
            typeof(bool), typeof(bool?),
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(decimal), typeof(decimal?),
            typeof(DateTime), typeof(DateTime?),
            typeof(DateTimeOffset), typeof(DateTimeOffset?),
            typeof(long), typeof(long?)
        };

        public static string BuildFieldSelection(Type contentType, HashSet<Type>? visited = null)
        {
            visited ??= new HashSet<Type>();
            if (!visited.Add(contentType))
                return string.Empty;

            var sb = new StringBuilder();
            var properties = contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                var fieldName = GetFieldName(prop);
                var propertyType = UnwrapNullable(prop.PropertyType);

                if (propertyType == typeof(GraphContentMetadata))
                {
                    sb.AppendLine("_metadata { key displayName types locale url { default hierarchical } published status created lastModified version }");
                }
                else if (propertyType == typeof(GraphContentRichText))
                {
                    sb.AppendLine($"{fieldName} {{ html }}");
                }
                else if (propertyType == typeof(GraphContentReference))
                {
                    sb.AppendLine($"{fieldName} {{ key url {{ default }} }}");
                }
                else if (propertyType == typeof(GraphContentUrl))
                {
                    sb.AppendLine($"{fieldName} {{ default hierarchical type base }}");
                }
                else if (propertyType == typeof(ContentComposition))
                {
                    // Composition handled separately via BuildCompositionSelection
                    continue;
                }
                else if (IsScalarType(propertyType))
                {
                    sb.AppendLine(fieldName);
                }
                else if (IsListOfScalars(propertyType))
                {
                    sb.AppendLine(fieldName);
                }
                else if (IsListOfComplex(propertyType, out var elementType))
                {
                    var subFields = BuildFieldSelection(elementType!, visited);
                    if (!string.IsNullOrWhiteSpace(subFields))
                        sb.AppendLine($"{fieldName} {{ {subFields.Trim()} }}");
                    else
                        sb.AppendLine(fieldName);
                }
                else
                {
                    // Complex object
                    var subFields = BuildFieldSelection(propertyType, visited);
                    if (!string.IsNullOrWhiteSpace(subFields))
                        sb.AppendLine($"{fieldName} {{ {subFields.Trim()} }}");
                    else
                        sb.AppendLine(fieldName);
                }
            }

            visited.Remove(contentType);
            return sb.ToString();
        }

        /// <summary>
        /// Builds field selection excluding _metadata (for use inside inline fragments
        /// where _metadata is already selected at the parent level).
        /// </summary>
        public static string BuildFieldSelectionWithoutMetadata(Type contentType)
        {
            var full = BuildFieldSelection(contentType);
            var lines = full.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.TrimStart().StartsWith("_metadata"))
                .ToList();
            return string.Join('\n', lines);
        }

        public static string BuildCompositionSelection(IEnumerable<Type> componentTypes, int depth = 3)
        {
            var sb = new StringBuilder();
            sb.AppendLine("composition {");
            sb.AppendLine("  nodeType displayTemplateKey");
            sb.AppendLine("  displaySettings { key value }");
            sb.AppendLine("  nodes {");
            BuildCompositionNodesRecursive(sb, componentTypes, depth, "    ");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static void BuildCompositionNodesRecursive(StringBuilder sb, IEnumerable<Type> componentTypes, int depth, string indent)
        {
            // CompositionStructureNode fragment
            sb.AppendLine($"{indent}... on CompositionStructureNode {{");
            sb.AppendLine($"{indent}  key nodeType displayName displayTemplateKey layoutType");
            sb.AppendLine($"{indent}  displaySettings {{ key value }}");
            if (depth > 1)
            {
                sb.AppendLine($"{indent}  nodes {{");
                BuildCompositionNodesRecursive(sb, componentTypes, depth - 1, indent + "    ");
                sb.AppendLine($"{indent}  }}");
            }
            sb.AppendLine($"{indent}}}");

            // CompositionComponentNode fragment
            sb.AppendLine($"{indent}... on CompositionComponentNode {{");
            sb.AppendLine($"{indent}  key displayName displayTemplateKey");
            sb.AppendLine($"{indent}  displaySettings {{ key value }}");
            sb.AppendLine($"{indent}  component {{");
            sb.AppendLine($"{indent}    _metadata {{ key types displayName }}");
            foreach (var componentType in componentTypes)
            {
                var fields = BuildFieldSelection(componentType);
                var trimmedFields = fields.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedFields))
                {
                    // Remove _metadata from component inline fragment since we already include it above
                    var lines = trimmedFields.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Where(l => !l.TrimStart().StartsWith("_metadata"))
                        .ToList();
                    if (lines.Count > 0)
                    {
                        sb.AppendLine($"{indent}    ... on {componentType.Name} {{ {string.Join(" ", lines.Select(l => l.Trim()))} }}");
                    }
                }
            }
            sb.AppendLine($"{indent}  }}");
            sb.AppendLine($"{indent}}}");
        }

        private static string GetFieldName(PropertyInfo prop)
        {
            var jsonAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            return jsonAttr?.Name ?? prop.Name;
        }

        private static Type UnwrapNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        private static bool IsScalarType(Type type)
        {
            type = UnwrapNullable(type);
            return _scalarTypes.Contains(type) || type.IsEnum;
        }

        private static bool IsListOfScalars(Type type)
        {
            var elementType = GetListElementType(type);
            return elementType != null && IsScalarType(elementType);
        }

        private static bool IsListOfComplex(Type type, out Type? elementType)
        {
            elementType = GetListElementType(type);
            return elementType != null && !IsScalarType(elementType);
        }

        private static Type? GetListElementType(Type type)
        {
            if (type == typeof(string))
                return null;

            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType)
            {
                var genDef = type.GetGenericTypeDefinition();
                if (genDef == typeof(List<>) || genDef == typeof(IList<>) ||
                    genDef == typeof(IEnumerable<>) || genDef == typeof(ICollection<>) ||
                    genDef == typeof(IReadOnlyList<>) || genDef == typeof(IReadOnlyCollection<>))
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return null;
        }
    }
}

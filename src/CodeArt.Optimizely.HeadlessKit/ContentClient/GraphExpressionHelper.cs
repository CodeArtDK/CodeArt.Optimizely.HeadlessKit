using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    internal static class GraphExpressionHelper
    {
        /// <summary>
        /// Resolves a member access expression to a dot-separated GraphQL field path,
        /// using [JsonPropertyName] attributes when present.
        /// Example: ap => ap.MetaData.Published  →  "_metadata.published"
        /// </summary>
        public static string ResolveFieldPath<T, TKey>(Expression<Func<T, TKey>> expression)
        {
            var members = new List<string>();
            var current = expression.Body;

            // Unwrap Convert nodes (boxing, e.g. DateTime? → object)
            while (current is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                current = unary.Operand;

            while (current is MemberExpression memberExpr)
            {
                var prop = memberExpr.Member;
                var jsonAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                members.Add(jsonAttr?.Name ?? prop.Name);
                current = memberExpr.Expression!;
            }

            if (members.Count == 0)
                throw new ArgumentException("Expression must be a member access expression (e.g. x => x.Property).");

            members.Reverse();
            return string.Join(".", members);
        }
    }
}

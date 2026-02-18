using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Text;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Generates GraphQL queries automatically from registered content types.
    /// Builds inline fragments for all page types and composition selections for component types.
    /// The generated query is lazily computed and cached.
    /// </summary>
    public class AutoGraphQueryProvider : IGraphQueryProvider
    {
        private readonly Lazy<string> _mainQuery;

        /// <summary>
        /// Initializes a new instance of <see cref="AutoGraphQueryProvider"/> using the specified content type registry.
        /// </summary>
        /// <param name="contentTypeRegistry">The registry providing page and component types for query generation.</param>
        public AutoGraphQueryProvider(IContentTypeRegistry contentTypeRegistry)
        {
            var registry = contentTypeRegistry ?? throw new ArgumentNullException(nameof(contentTypeRegistry));
            _mainQuery = new Lazy<string>(() => GenerateMainQuery(registry));
        }

        /// <summary>
        /// Returns the main query containing <c>MainContentQuery</c>, <c>GetContentByKey</c>,
        /// <c>GetChildrenByKey</c>, and <c>GetContentByKeyAndVersion</c> operations.
        /// </summary>
        /// <returns>A multi-operation GraphQL query string with inline fragments for all registered types.</returns>
        public string ProvideMainRelativePathQuery()
        {
            return _mainQuery.Value;
        }

        private static string GenerateMainQuery(IContentTypeRegistry registry)
        {
            var pageTypes = registry.PageTypes;
            var componentTypes = registry.ComponentTypes;

            var sb = new StringBuilder();

            // Build the inline fragments for all page types
            var pageFragments = BuildPageFragments(pageTypes);

            // Build composition selection for component types
            var compositionSelection = componentTypes.Count > 0
                ? GraphFieldMapper.BuildCompositionSelection(componentTypes)
                : string.Empty;

            // MainContentQuery — query by relative path
            sb.AppendLine("query MainContentQuery($path: String!, $locale: [Locales]) {");
            sb.AppendLine("  _Content(");
            sb.AppendLine("    where: { _metadata: { url: { default: { eq: $path } } } }");
            sb.AppendLine("    locale: $locale");
            sb.AppendLine("  ) {");
            sb.AppendLine("    items {");
            AppendContentFields(sb, pageFragments, compositionSelection, "      ");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine();

            // GetContentByKey
            sb.AppendLine("query GetContentByKey($key: String!, $locale: [Locales]) {");
            sb.AppendLine("  _Content(");
            sb.AppendLine("    where: { _metadata: { key: { eq: $key } } }");
            sb.AppendLine("    locale: $locale");
            sb.AppendLine("  ) {");
            sb.AppendLine("    items {");
            AppendContentFields(sb, pageFragments, compositionSelection, "      ");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine();

            // GetChildrenByKey — lightweight query (metadata only, no type-specific fragments)
            sb.AppendLine("query GetChildrenByKey($key: String!, $locale: [Locales]) {");
            sb.AppendLine("  _Content(");
            sb.AppendLine("    where: { _metadata: { key: { eq: $key } } }");
            sb.AppendLine("    locale: $locale");
            sb.AppendLine("  ) {");
            sb.AppendLine("    items {");
            sb.AppendLine("      _link {");
            sb.AppendLine("        _Content {");
            sb.AppendLine("          items {");
            sb.AppendLine("            __typename");
            sb.AppendLine("            _metadata { key displayName types locale url { default hierarchical } published status created lastModified version }");
            sb.AppendLine("          }");
            sb.AppendLine("        }");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine();

            // GetContentByKeyAndVersion (preview)
            sb.AppendLine("query GetContentByKeyAndVersion($key: String!, $version: String, $locale: [Locales]) {");
            sb.AppendLine("  _Content(");
            sb.AppendLine("    where: { _metadata: { key: { eq: $key }, version: { eq: $version } } }");
            sb.AppendLine("    locale: $locale");
            sb.AppendLine("  ) {");
            sb.AppendLine("    items {");
            AppendContentFields(sb, pageFragments, compositionSelection, "      ");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static void AppendContentFields(StringBuilder sb, string pageFragments, string compositionSelection, string indent)
        {
            sb.AppendLine($"{indent}__typename");
            sb.AppendLine($"{indent}_metadata {{ key displayName types locale url {{ default hierarchical }} published status created lastModified version }}");

            if (!string.IsNullOrWhiteSpace(pageFragments))
            {
                foreach (var line in pageFragments.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.AppendLine($"{indent}{line.TrimEnd()}");
                }
            }

            if (!string.IsNullOrWhiteSpace(compositionSelection))
            {
                // composition is only available on Experience/Section types, not _IContent
                sb.AppendLine($"{indent}... on _IExperience {{");
                foreach (var line in compositionSelection.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.AppendLine($"{indent}  {line.TrimEnd()}");
                }
                sb.AppendLine($"{indent}}}");
            }
        }

        private static string BuildPageFragments(IReadOnlyCollection<Type> pageTypes)
        {
            var sb = new StringBuilder();

            foreach (var pageType in pageTypes)
            {
                var fields = GraphFieldMapper.BuildFieldSelection(pageType);
                var trimmedFields = fields.Trim();
                if (string.IsNullOrWhiteSpace(trimmedFields))
                    continue;

                // Remove _metadata from inline fragments since we already include it at the top level
                var lines = trimmedFields.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => !l.TrimStart().StartsWith("_metadata"))
                    .Select(l => l.Trim())
                    .ToList();

                if (lines.Count > 0)
                {
                    sb.AppendLine($"... on {pageType.Name} {{ {string.Join(" ", lines)} }}");
                }
            }

            return sb.ToString();
        }
    }
}

using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Linq.Expressions;
using System.Text;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public static partial class GraphQuery
    {
        public static SearchQueryBuilder<T> Search<T>() where T : class, IGraphContent, new()
        {
            return new SearchQueryBuilder<T>();
        }

        public static SearchQueryBuilder<T> Search<T>(ContentGraphClient client) where T : class, IGraphContent, new()
        {
            return new SearchQueryBuilder<T>(client);
        }

        /// <summary>
        /// Search across all URL-addressable page types (_Page).
        /// Results are deserialized as <typeparamref name="T"/>; properties not present
        /// on a particular page type will be null.
        /// </summary>
        public static SearchQueryBuilder<T> SearchPages<T>(ContentGraphClient client) where T : class, IGraphContent, new()
        {
            return new SearchQueryBuilder<T>(client).ForType("_Page");
        }
    }

    public class SearchQueryBuilder<T> where T : class, IGraphContent, new()
    {
        private ContentGraphClient? _client;
        private IContentTypeRegistry? _registry;
        private string? _typeName;
        private readonly List<string> _locales = new();
        private int? _skip;
        private int? _take;
        private string? _orderByField;
        private OrderDirection? _orderDirection;

        // Fulltext parameters
        private string? _matchText;
        private bool _fuzzy;
        private int? _boost;
        private readonly List<string> _facetFields = new();
        private readonly List<(string field, int fragmentSize)> _highlights = new();

        public SearchQueryBuilder() { }

        public SearchQueryBuilder(ContentGraphClient client)
        {
            _client = client;
        }

        public SearchQueryBuilder<T> WithClient(ContentGraphClient client)
        {
            _client = client;
            return this;
        }

        public SearchQueryBuilder<T> WithRegistry(IContentTypeRegistry registry)
        {
            _registry = registry;
            return this;
        }

        /// <summary>
        /// Override the GraphQL type name used in the query (e.g. "_Page" to search all pages).
        /// </summary>
        public SearchQueryBuilder<T> ForType(string typeName)
        {
            _typeName = typeName;
            return this;
        }

        public SearchQueryBuilder<T> Match(string text)
        {
            _matchText = text;
            return this;
        }

        public SearchQueryBuilder<T> Fuzzy(string text)
        {
            _matchText = text;
            _fuzzy = true;
            return this;
        }

        /// <summary>
        /// Set the fulltext relevance boost factor.
        /// </summary>
        public SearchQueryBuilder<T> Boost(int weight)
        {
            _boost = weight;
            return this;
        }

        public SearchQueryBuilder<T> Facet(string field)
        {
            _facetFields.Add(field);
            return this;
        }

        public SearchQueryBuilder<T> Highlight(string field, int fragmentSize = 200)
        {
            _highlights.Add((field, fragmentSize));
            return this;
        }

        public SearchQueryBuilder<T> Locale(params string[] locales)
        {
            _locales.AddRange(locales);
            return this;
        }

        public SearchQueryBuilder<T> Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        public SearchQueryBuilder<T> Take(int take)
        {
            _take = take;
            return this;
        }

        public SearchQueryBuilder<T> OrderBy(string field, OrderDirection direction = OrderDirection.ASC)
        {
            _orderByField = field;
            _orderDirection = direction;
            return this;
        }

        public SearchQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, OrderDirection direction = OrderDirection.ASC)
        {
            _orderByField = GraphExpressionHelper.ResolveFieldPath(keySelector);
            _orderDirection = direction;
            return this;
        }

        internal string QueryTypeName => _typeName ?? typeof(T).Name;
        private bool UseInlineFragment => _typeName != null;

        public string Build()
        {
            var typeName = QueryTypeName;
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.Append($"  {typeName}(");

            var args = new List<string>();

            // Where clause with fulltext
            if (!string.IsNullOrWhiteSpace(_matchText))
            {
                var escapedText = EscapeGraphQL(_matchText);
                var fulltextParts = new List<string>();
                fulltextParts.Add($"match: \"{escapedText}\"");

                if (_fuzzy)
                    fulltextParts.Add("fuzzy: true");

                if (_boost.HasValue)
                    fulltextParts.Add($"boost: {_boost.Value}");

                args.Add($"where: {{ _fulltext: {{ {string.Join(", ", fulltextParts)} }} }}");
            }

            // Locale
            if (_locales.Count > 0)
            {
                var localeList = string.Join(", ", _locales);
                args.Add($"locale: [{localeList}]");
            }

            // Pagination
            if (_take.HasValue)
                args.Add($"limit: {_take.Value}");
            if (_skip.HasValue)
                args.Add($"skip: {_skip.Value}");

            // Order by
            if (_orderByField != null && _orderDirection.HasValue)
            {
                args.Add($"orderBy: {{ {OrderByHelper.BuildOrderByClause(_orderByField, _orderDirection.Value)} }}");
            }

            sb.Append(string.Join(", ", args));
            sb.AppendLine(") {");

            // Items — use inline fragment for union types (ForType), direct fields otherwise
            sb.AppendLine("    items {");
            if (UseInlineFragment)
            {
                sb.AppendLine("      _metadata { key displayName types locale url { default hierarchical } published status created lastModified version }");
                var fragmentFields = GraphFieldMapper.BuildFieldSelectionWithoutMetadata(typeof(T));
                if (!string.IsNullOrWhiteSpace(fragmentFields))
                {
                    sb.Append($"      ... on {typeof(T).Name} {{ ");
                    sb.Append(string.Join(" ", fragmentFields.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim())));
                    sb.AppendLine(" }");
                }
            }
            else
            {
                var fields = GraphFieldMapper.BuildFieldSelection(typeof(T));
                foreach (var line in fields.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.AppendLine($"      {line.Trim()}");
                }
            }
            sb.AppendLine("    }");

            // Facets
            foreach (var facetField in _facetFields)
            {
                sb.AppendLine($"    facets {{");
                sb.AppendLine($"      {facetField} {{");
                sb.AppendLine($"        name");
                sb.AppendLine($"        count");
                sb.AppendLine($"      }}");
                sb.AppendLine($"    }}");
            }

            // Autocomplete (highlights)
            if (_highlights.Count > 0)
            {
                sb.AppendLine("    autocomplete {");
                foreach (var (field, fragmentSize) in _highlights)
                {
                    sb.AppendLine($"      {field}");
                }
                sb.AppendLine("    }");
            }

            sb.AppendLine("    total");
            sb.AppendLine("    cursor");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public async Task<SearchResult<T>> ExecuteAsync()
        {
            if (_client == null)
                throw new InvalidOperationException("No ContentGraphClient configured. Use WithClient() or GraphQuery.Search<T>(client).");

            var query = Build();
            var result = await _client.ExecuteQueryAsync<T>(query, QueryTypeName);
            return new SearchResult<T>
            {
                Items = result.Items,
                Total = result.Total,
                Cursor = result.Cursor
            };
        }

        private static string EscapeGraphQL(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}

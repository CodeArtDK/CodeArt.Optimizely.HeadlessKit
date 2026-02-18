using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Linq.Expressions;
using System.Text;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public static partial class GraphQuery
    {
        /// <summary>
        /// Creates a new <see cref="SearchQueryBuilder{T}"/> for full-text search queries.
        /// A client must be set via <see cref="SearchQueryBuilder{T}.WithClient"/> before execution.
        /// </summary>
        /// <typeparam name="T">The content model type to search for.</typeparam>
        /// <returns>A new <see cref="SearchQueryBuilder{T}"/>.</returns>
        public static SearchQueryBuilder<T> Search<T>() where T : class, IGraphContent, new()
        {
            return new SearchQueryBuilder<T>();
        }

        /// <summary>
        /// Creates a new <see cref="SearchQueryBuilder{T}"/> pre-configured with a client for immediate execution.
        /// </summary>
        /// <typeparam name="T">The content model type to search for.</typeparam>
        /// <param name="client">The <see cref="ContentGraphClient"/> to use for query execution.</param>
        /// <returns>A new <see cref="SearchQueryBuilder{T}"/> ready for building and executing searches.</returns>
        public static SearchQueryBuilder<T> Search<T>(ContentGraphClient client) where T : class, IGraphContent, new()
        {
            return new SearchQueryBuilder<T>(client);
        }

        /// <summary>
        /// Creates a search builder targeting all URL-addressable page types (<c>_Page</c>).
        /// Results are deserialized as <typeparamref name="T"/>; properties not present
        /// on a particular page type will be <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The base content model type to deserialize results into.</typeparam>
        /// <param name="client">The <see cref="ContentGraphClient"/> to use for query execution.</param>
        /// <returns>A new <see cref="SearchQueryBuilder{T}"/> targeting the <c>_Page</c> union type.</returns>
        public static SearchQueryBuilder<T> SearchPages<T>(ContentGraphClient client) where T : class, IGraphContent, new()
        {
            return new SearchQueryBuilder<T>(client).ForType("_Page");
        }
    }

    /// <summary>
    /// Fluent builder for constructing full-text search queries against Optimizely Graph.
    /// Supports exact and fuzzy matching, facets, highlights, pagination, and ordering.
    /// </summary>
    /// <typeparam name="T">The content model type to search for.</typeparam>
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

        /// <summary>
        /// Initializes a new <see cref="SearchQueryBuilder{T}"/> without a client.
        /// A client must be set via <see cref="WithClient"/> before execution.
        /// </summary>
        public SearchQueryBuilder() { }

        /// <summary>
        /// Initializes a new <see cref="SearchQueryBuilder{T}"/> with a client for query execution.
        /// </summary>
        /// <param name="client">The <see cref="ContentGraphClient"/> to use for query execution.</param>
        public SearchQueryBuilder(ContentGraphClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Sets the <see cref="ContentGraphClient"/> for query execution.
        /// </summary>
        /// <param name="client">The client to use.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> WithClient(ContentGraphClient client)
        {
            _client = client;
            return this;
        }

        /// <summary>
        /// Sets the content type registry for composition type resolution.
        /// </summary>
        /// <param name="registry">The content type registry to use.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> WithRegistry(IContentTypeRegistry registry)
        {
            _registry = registry;
            return this;
        }

        /// <summary>
        /// Overrides the GraphQL type name used in the query (e.g. "_Page" to search all page types).
        /// </summary>
        /// <param name="typeName">The GraphQL type name to search against.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> ForType(string typeName)
        {
            _typeName = typeName;
            return this;
        }

        /// <summary>
        /// Sets the full-text search term for exact matching.
        /// </summary>
        /// <param name="text">The search text.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Match(string text)
        {
            _matchText = text;
            return this;
        }

        /// <summary>
        /// Sets the full-text search term with fuzzy matching, which tolerates typos and near-matches.
        /// </summary>
        /// <param name="text">The search text.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Fuzzy(string text)
        {
            _matchText = text;
            _fuzzy = true;
            return this;
        }

        /// <summary>
        /// Sets the relevance boost factor for full-text search scoring.
        /// </summary>
        /// <param name="weight">The boost weight value.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Boost(int weight)
        {
            _boost = weight;
            return this;
        }

        /// <summary>
        /// Adds a facet field for aggregated counts in the search results.
        /// </summary>
        /// <param name="field">The field name to facet on.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Facet(string field)
        {
            _facetFields.Add(field);
            return this;
        }

        /// <summary>
        /// Adds a highlight field for search result snippets.
        /// </summary>
        /// <param name="field">The field name to highlight.</param>
        /// <param name="fragmentSize">The maximum fragment size in characters. Defaults to 200.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Highlight(string field, int fragmentSize = 200)
        {
            _highlights.Add((field, fragmentSize));
            return this;
        }

        /// <summary>
        /// Sets the query locale(s).
        /// </summary>
        /// <param name="locales">One or more locale codes.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Locale(params string[] locales)
        {
            _locales.AddRange(locales);
            return this;
        }

        /// <summary>
        /// Sets the number of items to skip for pagination.
        /// </summary>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of items to return.
        /// </summary>
        /// <param name="take">The maximum number of items.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> Take(int take)
        {
            _take = take;
            return this;
        }

        /// <summary>
        /// Orders results by a field path string.
        /// </summary>
        /// <param name="field">The dotted field path to order by.</param>
        /// <param name="direction">The sort direction. Defaults to <see cref="OrderDirection.ASC"/>.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> OrderBy(string field, OrderDirection direction = OrderDirection.ASC)
        {
            _orderByField = field;
            _orderDirection = direction;
            return this;
        }

        /// <summary>
        /// Orders results by a strongly-typed property expression.
        /// </summary>
        /// <typeparam name="TKey">The type of the property to order by.</typeparam>
        /// <param name="keySelector">A lambda expression selecting the property to order by.</param>
        /// <param name="direction">The sort direction. Defaults to <see cref="OrderDirection.ASC"/>.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SearchQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, OrderDirection direction = OrderDirection.ASC)
        {
            _orderByField = GraphExpressionHelper.ResolveFieldPath(keySelector);
            _orderDirection = direction;
            return this;
        }

        internal string QueryTypeName => _typeName ?? typeof(T).Name;
        private bool UseInlineFragment => _typeName != null;

        /// <summary>
        /// Builds and returns the search GraphQL query string from the current builder configuration.
        /// </summary>
        /// <returns>The constructed GraphQL search query string.</returns>
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

        /// <summary>
        /// Executes the search query and returns a <see cref="SearchResult{T}"/> containing items, total count, and optional facets.
        /// </summary>
        /// <returns>The search result.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="ContentGraphClient"/> has been configured.</exception>
        /// <exception cref="GraphQueryException">Thrown when the GraphQL query returns errors.</exception>
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

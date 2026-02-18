using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Specifies the sort direction for query results.
    /// </summary>
    public enum OrderDirection
    {
        /// <summary>Ascending order (A-Z, oldest first, lowest first).</summary>
        ASC,
        /// <summary>Descending order (Z-A, newest first, highest first).</summary>
        DESC
    }

    /// <summary>
    /// Entry point for creating fluent content queries against Optimizely Graph.
    /// </summary>
    /// <example>
    /// <code>
    /// // Create a query builder for a specific content type
    /// var builder = GraphQuery.For&lt;StandardPage&gt;(client);
    ///
    /// // Build and execute a filtered, paginated query
    /// var pages = await GraphQuery.For&lt;StandardPage&gt;(client)
    ///     .Where(f => f.Metadata.Status.Eq("Published"))
    ///     .Take(10)
    ///     .ToListAsync();
    /// </code>
    /// </example>
    public static partial class GraphQuery
    {
        /// <summary>
        /// Creates a new <see cref="GraphQueryBuilder{T}"/> for the specified content type.
        /// A client must be set via <see cref="GraphQueryBuilder{T}.WithClient"/> before execution.
        /// </summary>
        /// <typeparam name="T">The content model type to query for.</typeparam>
        /// <returns>A new <see cref="GraphQueryBuilder{T}"/>.</returns>
        public static GraphQueryBuilder<T> For<T>() where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>();
        }

        /// <summary>
        /// Creates a new <see cref="GraphQueryBuilder{T}"/> pre-configured with a client for immediate execution.
        /// </summary>
        /// <typeparam name="T">The content model type to query for.</typeparam>
        /// <param name="client">The <see cref="ContentGraphClient"/> to use for query execution.</param>
        /// <returns>A new <see cref="GraphQueryBuilder{T}"/> ready for building and executing queries.</returns>
        public static GraphQueryBuilder<T> For<T>(ContentGraphClient client) where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>(client);
        }

        /// <summary>
        /// Creates a new <see cref="GraphQueryBuilder{T}"/> pre-configured with a client and content type registry.
        /// </summary>
        /// <typeparam name="T">The content model type to query for.</typeparam>
        /// <param name="client">The <see cref="ContentGraphClient"/> to use for query execution.</param>
        /// <param name="registry">The content type registry for resolving composition types.</param>
        /// <returns>A new <see cref="GraphQueryBuilder{T}"/> ready for building and executing queries.</returns>
        public static GraphQueryBuilder<T> For<T>(ContentGraphClient client, IContentTypeRegistry registry)
            where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>(client, registry);
        }
    }

    /// <summary>
    /// Fluent builder for constructing typed GraphQL content queries against Optimizely Graph.
    /// Supports filtering, pagination, ordering, locale selection, and composition inclusion.
    /// </summary>
    /// <typeparam name="T">The content model type to query for.</typeparam>
    /// <example>
    /// <code>
    /// var pages = await GraphQuery.For&lt;StandardPage&gt;(client)
    ///     .Where(f => f.Metadata.Status.Eq("Published"))
    ///     .OrderBy(p => p.Metadata.Published, OrderDirection.DESC)
    ///     .Take(10)
    ///     .ToListAsync();
    /// </code>
    /// </example>
    public class GraphQueryBuilder<T> where T : class, IGraphContent, new()
    {
        private ContentGraphClient? _client;
        private IContentTypeRegistry? _registry;
        private string? _typeName;
        private readonly List<GraphFilter> _filters = new();
        private readonly List<string> _locales = new();
        private int? _skip;
        private int? _take;
        private string? _orderByField;
        private OrderDirection? _orderDirection;
        private bool _includeComposition;
        private int _compositionDepth = 3;
        private string? _cursor;

        /// <summary>
        /// Initializes a new <see cref="GraphQueryBuilder{T}"/> without a client.
        /// A client must be set via <see cref="WithClient"/> before query execution.
        /// </summary>
        public GraphQueryBuilder() { }

        /// <summary>
        /// Initializes a new <see cref="GraphQueryBuilder{T}"/> with a client for query execution.
        /// </summary>
        /// <param name="client">The <see cref="ContentGraphClient"/> to use for query execution.</param>
        public GraphQueryBuilder(ContentGraphClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Initializes a new <see cref="GraphQueryBuilder{T}"/> with a client and content type registry.
        /// </summary>
        /// <param name="client">The <see cref="ContentGraphClient"/> to use for query execution.</param>
        /// <param name="registry">The content type registry for resolving composition types.</param>
        public GraphQueryBuilder(ContentGraphClient client, IContentTypeRegistry registry)
        {
            _client = client;
            _registry = registry;
        }

        /// <summary>
        /// Sets the <see cref="ContentGraphClient"/> for query execution.
        /// </summary>
        /// <param name="client">The client to use.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> WithClient(ContentGraphClient client)
        {
            _client = client;
            return this;
        }

        /// <summary>
        /// Sets the content type registry for composition type resolution.
        /// </summary>
        /// <param name="registry">The content type registry to use.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> WithRegistry(IContentTypeRegistry registry)
        {
            _registry = registry;
            return this;
        }

        /// <summary>
        /// Overrides the GraphQL type name used in the query (e.g. "_Page" for union queries across all page types).
        /// </summary>
        /// <param name="typeName">The GraphQL type name to query against.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> ForType(string typeName)
        {
            _typeName = typeName;
            return this;
        }

        /// <summary>
        /// Adds a filter using a builder expression.
        /// Multiple <see cref="Where(Func{GraphFilterBuilder, GraphFilter})"/> calls are combined with AND logic.
        /// </summary>
        /// <param name="filterExpression">A function that receives a <see cref="GraphFilterBuilder"/> and returns a <see cref="GraphFilter"/>.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> Where(Func<GraphFilterBuilder, GraphFilter> filterExpression)
        {
            var builder = new GraphFilterBuilder();
            _filters.Add(filterExpression(builder));
            return this;
        }

        /// <summary>
        /// Adds a pre-built <see cref="GraphFilter"/> to the query.
        /// Multiple filters are combined with AND logic.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> Where(GraphFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        /// <summary>
        /// Sets the query locale(s). Use Optimizely locale codes (e.g. "en", "sv") or "ALL" for all locales.
        /// </summary>
        /// <param name="locales">One or more locale codes.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> Locale(params string[] locales)
        {
            _locales.AddRange(locales);
            return this;
        }

        /// <summary>
        /// Sets the number of items to skip for offset-based pagination.
        /// </summary>
        /// <param name="skip">The number of items to skip.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of items to return.
        /// </summary>
        /// <param name="take">The maximum number of items.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> Take(int take)
        {
            _take = take;
            return this;
        }

        /// <summary>
        /// Orders results by a field path string (e.g. "_metadata.published").
        /// </summary>
        /// <param name="field">The dotted field path to order by.</param>
        /// <param name="direction">The sort direction. Defaults to <see cref="OrderDirection.ASC"/>.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> OrderBy(string field, OrderDirection direction = OrderDirection.ASC)
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
        public GraphQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, OrderDirection direction = OrderDirection.ASC)
        {
            _orderByField = GraphExpressionHelper.ResolveFieldPath(keySelector);
            _orderDirection = direction;
            return this;
        }

        /// <summary>
        /// Includes composition data (visual builder structure) in the query results.
        /// Requires a content type registry to be configured.
        /// </summary>
        /// <param name="depth">The nesting depth for composition node traversal. Defaults to 3.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> WithComposition(int depth = 3)
        {
            _includeComposition = true;
            _compositionDepth = depth;
            return this;
        }

        /// <summary>
        /// Sets the cursor for cursor-based pagination. Use the <see cref="GraphQueryResult{T}.Cursor"/>
        /// from a previous result to fetch the next page.
        /// </summary>
        /// <param name="cursor">The pagination cursor string.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> After(string cursor)
        {
            _cursor = cursor;
            return this;
        }

        /// <summary>
        /// Shorthand filter for <c>_metadata.url.default</c>. Equivalent to
        /// <c>Where(f => f.Metadata.Url.Default.Eq(url))</c>.
        /// </summary>
        /// <param name="url">The URL to filter by.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> ForUrl(string url)
        {
            return Where(f => f.Metadata.Url.Default.Eq(url));
        }

        /// <summary>
        /// Shorthand filter for <c>_metadata.key</c>. Equivalent to
        /// <c>Where(f => f.Metadata.Key.Eq(key))</c>.
        /// </summary>
        /// <param name="key">The content key to filter by.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public GraphQueryBuilder<T> ForKey(string key)
        {
            return Where(f => f.Metadata.Key.Eq(key));
        }

        internal string QueryTypeName => _typeName ?? typeof(T).Name;
        private bool UseInlineFragment => _typeName != null;

        /// <summary>
        /// Builds and returns the GraphQL query string from the current builder configuration.
        /// </summary>
        /// <returns>The constructed GraphQL query string.</returns>
        public string Build()
        {
            var typeName = QueryTypeName;
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.Append($"  {typeName}(");

            var args = new List<string>();

            // Where clause
            if (_filters.Count > 0)
            {
                var whereClause = _filters.Count == 1
                    ? _filters[0].ToString()
                    : string.Join(", ", _filters.Select(f => $"{{ {f} }}"));

                if (_filters.Count > 1)
                    args.Add($"where: {{ _and: [{whereClause}] }}");
                else
                    args.Add($"where: {{ {whereClause} }}");
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
            if (_cursor != null)
                args.Add($"cursor: \"{_cursor}\"");

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

                // Composition
                if (_includeComposition && _registry != null)
                {
                    var compositionFields = GraphFieldMapper.BuildCompositionSelection(_registry.ComponentTypes, _compositionDepth);
                    foreach (var line in compositionFields.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                    {
                        sb.AppendLine($"      {line.Trim()}");
                    }
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("    total");
            sb.AppendLine("    cursor");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Executes the query and returns a <see cref="GraphQueryResult{T}"/> containing items, total count, and cursor.
        /// </summary>
        /// <returns>The query result.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="ContentGraphClient"/> has been configured.</exception>
        /// <exception cref="GraphQueryException">Thrown when the GraphQL query returns errors.</exception>
        public async Task<GraphQueryResult<T>> ExecuteAsync()
        {
            if (_client == null)
                throw new InvalidOperationException("No ContentGraphClient configured. Use WithClient() or GraphQuery.For<T>(client).");

            var query = Build();
            return await _client.ExecuteQueryAsync<T>(query, QueryTypeName);
        }

        /// <summary>
        /// Executes the query and returns the matched items as a <see cref="List{T}"/>.
        /// </summary>
        /// <returns>A list of matched content items.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="ContentGraphClient"/> has been configured.</exception>
        public async Task<List<T>> ToListAsync()
        {
            var result = await ExecuteAsync();
            return result.Items;
        }

        /// <summary>
        /// Executes the query with <see cref="Take"/> set to 1 and returns the first matched item, or <c>null</c> if none found.
        /// </summary>
        /// <returns>The first matching content item, or <c>null</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="ContentGraphClient"/> has been configured.</exception>
        public async Task<T?> FirstOrDefaultAsync()
        {
            _take = 1;
            var result = await ExecuteAsync();
            return result.Items.FirstOrDefault();
        }

        /// <summary>
        /// Executes the query and returns a <see cref="PagedResult{T}"/> with items, total count,
        /// pagination cursor, and a <see cref="PagedResult{T}.HasMore"/> flag.
        /// </summary>
        /// <returns>A paged result with pagination metadata.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="ContentGraphClient"/> has been configured.</exception>
        public async Task<PagedResult<T>> ToPagedResultAsync()
        {
            var result = await ExecuteAsync();
            return new PagedResult<T>
            {
                Items = result.Items,
                Total = result.Total,
                Cursor = result.Cursor,
                HasMore = !string.IsNullOrEmpty(result.Cursor) && result.Items.Count > 0 && result.Items.Count < result.Total
            };
        }

        /// <summary>
        /// Returns an <see cref="IAsyncEnumerable{T}"/> that automatically pages through all matching results.
        /// Each page is fetched lazily as the enumerable is consumed.
        /// </summary>
        /// <param name="pageSize">The number of items to fetch per page. Defaults to 20.</param>
        /// <param name="cancellationToken">A cancellation token to stop enumeration.</param>
        /// <returns>An async enumerable of all matching content items.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="ContentGraphClient"/> has been configured.</exception>
        public async IAsyncEnumerable<T> ToAsyncEnumerable(int pageSize = 20, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (_client == null)
                throw new InvalidOperationException("No ContentGraphClient configured. Use WithClient() or GraphQuery.For<T>(client).");

            _take = pageSize;
            string? cursor = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                _cursor = cursor;
                var query = Build();
                var result = await _client.ExecuteQueryAsync<T>(query, QueryTypeName);

                foreach (var item in result.Items)
                {
                    yield return item;
                }

                if (result.Items.Count < pageSize || string.IsNullOrEmpty(result.Cursor))
                    yield break;

                cursor = result.Cursor;
            }
        }
    }

    /// <summary>
    /// Contains a page of query results with pagination metadata.
    /// </summary>
    /// <typeparam name="T">The content model type.</typeparam>
    public class PagedResult<T> where T : class, IGraphContent
    {
        /// <summary>Gets or sets the content items for the current page.</summary>
        public List<T> Items { get; set; } = new();

        /// <summary>Gets or sets the total number of matching items across all pages.</summary>
        public int Total { get; set; }

        /// <summary>Gets or sets the cursor for fetching the next page. <c>null</c> if no more pages.</summary>
        public string? Cursor { get; set; }

        /// <summary>Gets or sets a value indicating whether more pages are available.</summary>
        public bool HasMore { get; set; }
    }

    internal static class OrderByHelper
    {
        /// <summary>
        /// Builds a nested GraphQL orderBy clause from a dotted field path.
        /// Example: "_metadata.published" + DESC  →  "_metadata: { published: DESC }"
        /// </summary>
        internal static string BuildOrderByClause(string field, OrderDirection direction)
        {
            var parts = field.Split('.');
            var clause = $"{parts[^1]}: {direction}";
            for (int i = parts.Length - 2; i >= 0; i--)
            {
                clause = $"{parts[i]}: {{ {clause} }}";
            }
            return clause;
        }
    }
}

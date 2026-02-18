using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public enum OrderDirection
    {
        ASC,
        DESC
    }

    public static partial class GraphQuery
    {
        public static GraphQueryBuilder<T> For<T>() where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>();
        }

        public static GraphQueryBuilder<T> For<T>(ContentGraphClient client) where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>(client);
        }

        public static GraphQueryBuilder<T> For<T>(ContentGraphClient client, IContentTypeRegistry registry)
            where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>(client, registry);
        }
    }

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

        public GraphQueryBuilder() { }

        public GraphQueryBuilder(ContentGraphClient client)
        {
            _client = client;
        }

        public GraphQueryBuilder(ContentGraphClient client, IContentTypeRegistry registry)
        {
            _client = client;
            _registry = registry;
        }

        public GraphQueryBuilder<T> WithClient(ContentGraphClient client)
        {
            _client = client;
            return this;
        }

        public GraphQueryBuilder<T> WithRegistry(IContentTypeRegistry registry)
        {
            _registry = registry;
            return this;
        }

        /// <summary>
        /// Override the GraphQL type name used in the query (e.g. "_Page" to query all pages).
        /// </summary>
        public GraphQueryBuilder<T> ForType(string typeName)
        {
            _typeName = typeName;
            return this;
        }

        public GraphQueryBuilder<T> Where(Func<GraphFilterBuilder, GraphFilter> filterExpression)
        {
            var builder = new GraphFilterBuilder();
            _filters.Add(filterExpression(builder));
            return this;
        }

        public GraphQueryBuilder<T> Where(GraphFilter filter)
        {
            _filters.Add(filter);
            return this;
        }

        public GraphQueryBuilder<T> Locale(params string[] locales)
        {
            _locales.AddRange(locales);
            return this;
        }

        public GraphQueryBuilder<T> Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        public GraphQueryBuilder<T> Take(int take)
        {
            _take = take;
            return this;
        }

        public GraphQueryBuilder<T> OrderBy(string field, OrderDirection direction = OrderDirection.ASC)
        {
            _orderByField = field;
            _orderDirection = direction;
            return this;
        }

        public GraphQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, OrderDirection direction = OrderDirection.ASC)
        {
            _orderByField = GraphExpressionHelper.ResolveFieldPath(keySelector);
            _orderDirection = direction;
            return this;
        }

        public GraphQueryBuilder<T> WithComposition(int depth = 3)
        {
            _includeComposition = true;
            _compositionDepth = depth;
            return this;
        }

        public GraphQueryBuilder<T> After(string cursor)
        {
            _cursor = cursor;
            return this;
        }

        // Convenience methods
        public GraphQueryBuilder<T> ForUrl(string url)
        {
            return Where(f => f.Metadata.Url.Default.Eq(url));
        }

        public GraphQueryBuilder<T> ForKey(string key)
        {
            return Where(f => f.Metadata.Key.Eq(key));
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

        // Execution methods
        public async Task<GraphQueryResult<T>> ExecuteAsync()
        {
            if (_client == null)
                throw new InvalidOperationException("No ContentGraphClient configured. Use WithClient() or GraphQuery.For<T>(client).");

            var query = Build();
            return await _client.ExecuteQueryAsync<T>(query, QueryTypeName);
        }

        public async Task<List<T>> ToListAsync()
        {
            var result = await ExecuteAsync();
            return result.Items;
        }

        public async Task<T?> FirstOrDefaultAsync()
        {
            _take = 1;
            var result = await ExecuteAsync();
            return result.Items.FirstOrDefault();
        }

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

    public class PagedResult<T> where T : class, IGraphContent
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public string? Cursor { get; set; }
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

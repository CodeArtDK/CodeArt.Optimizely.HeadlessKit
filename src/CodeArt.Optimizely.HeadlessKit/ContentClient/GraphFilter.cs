using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public class GraphFilter
    {
        private readonly string _graphql;

        internal GraphFilter(string graphql)
        {
            _graphql = graphql;
        }

        public override string ToString() => _graphql;

        // Boolean composition
        public static GraphFilter And(params GraphFilter[] filters)
        {
            var parts = string.Join(", ", filters.Select(f => $"{{ {f} }}"));
            return new GraphFilter($"_and: [{parts}]");
        }

        public static GraphFilter Or(params GraphFilter[] filters)
        {
            var parts = string.Join(", ", filters.Select(f => $"{{ {f} }}"));
            return new GraphFilter($"_or: [{parts}]");
        }

        public static GraphFilter Not(GraphFilter filter)
        {
            return new GraphFilter($"_not: {{ {filter} }}");
        }
    }

    public class GraphFilterBuilder
    {
        public GraphFieldFilter Field(string fieldName) => new GraphFieldFilter(fieldName);

        public GraphMetadataFilter Metadata => new GraphMetadataFilter();

        public GraphFilter And(params GraphFilter[] filters) => GraphFilter.And(filters);
        public GraphFilter Or(params GraphFilter[] filters) => GraphFilter.Or(filters);
        public GraphFilter Not(GraphFilter filter) => GraphFilter.Not(filter);

        // Fulltext filter
        public GraphFulltextFilter Fulltext => new GraphFulltextFilter();
    }

    public class GraphFieldFilter
    {
        private readonly string _fieldPath;

        internal GraphFieldFilter(string fieldPath)
        {
            _fieldPath = fieldPath;
        }

        public GraphFilter Eq(string value) => Wrap("eq", FormatString(value));
        public GraphFilter Eq(int value) => Wrap("eq", value.ToString());
        public GraphFilter Eq(bool value) => Wrap("eq", value.ToString().ToLowerInvariant());
        public GraphFilter Eq(DateTime value) => Wrap("eq", FormatString(value.ToString("O")));

        public GraphFilter NotEq(string value) => Wrap("notEq", FormatString(value));
        public GraphFilter NotEq(int value) => Wrap("notEq", value.ToString());

        public GraphFilter Like(string value) => Wrap("like", FormatString(value));
        public GraphFilter StartsWith(string value) => Wrap("startsWith", FormatString(value));
        public GraphFilter EndsWith(string value) => Wrap("endsWith", FormatString(value));
        public GraphFilter Contains(string value) => Wrap("contains", FormatString(value));
        public GraphFilter Exists(bool exists) => Wrap("exist", exists.ToString().ToLowerInvariant());

        public GraphFilter In(params string[] values)
        {
            var formatted = string.Join(", ", values.Select(v => FormatString(v)));
            return Wrap("in", $"[{formatted}]");
        }

        public GraphFilter In(params int[] values)
        {
            var formatted = string.Join(", ", values);
            return Wrap("in", $"[{formatted}]");
        }

        public GraphFilter NotIn(params string[] values)
        {
            var formatted = string.Join(", ", values.Select(v => FormatString(v)));
            return Wrap("notIn", $"[{formatted}]");
        }

        public GraphFilter Gt(int value) => Wrap("gt", value.ToString());
        public GraphFilter Gte(int value) => Wrap("gte", value.ToString());
        public GraphFilter Lt(int value) => Wrap("lt", value.ToString());
        public GraphFilter Lte(int value) => Wrap("lte", value.ToString());

        public GraphFilter Gt(DateTime value) => Wrap("gt", FormatString(value.ToString("O")));
        public GraphFilter Gte(DateTime value) => Wrap("gte", FormatString(value.ToString("O")));
        public GraphFilter Lt(DateTime value) => Wrap("lt", FormatString(value.ToString("O")));
        public GraphFilter Lte(DateTime value) => Wrap("lte", FormatString(value.ToString("O")));

        private GraphFilter Wrap(string op, string value)
        {
            var parts = _fieldPath.Split('.');
            var result = $"{op}: {value}";
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                result = $"{parts[i]}: {{ {result} }}";
            }
            return new GraphFilter(result);
        }

        private static string FormatString(string value)
        {
            return $"\"{EscapeGraphQL(value)}\"";
        }

        private static string EscapeGraphQL(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }

    public class GraphMetadataFilter
    {
        public GraphFieldFilter Key => new GraphFieldFilter("_metadata.key");
        public GraphMetadataUrlFilter Url => new GraphMetadataUrlFilter();
        public GraphFieldFilter Status => new GraphFieldFilter("_metadata.status");
        public GraphFieldFilter Types => new GraphFieldFilter("_metadata.types");
        public GraphFieldFilter DisplayName => new GraphFieldFilter("_metadata.displayName");
        public GraphFieldFilter Published => new GraphFieldFilter("_metadata.published");
        public GraphFieldFilter Locale => new GraphFieldFilter("_metadata.locale");
    }

    public class GraphMetadataUrlFilter
    {
        public GraphFieldFilter Default => new GraphFieldFilter("_metadata.url.default");
        public GraphFieldFilter Hierarchical => new GraphFieldFilter("_metadata.url.hierarchical");
        public GraphFieldFilter Base => new GraphFieldFilter("_metadata.url.base");
        public GraphFieldFilter Internal => new GraphFieldFilter("_metadata.url.internal");
    }

    public class GraphFulltextFilter
    {
        public GraphFilter Match(string value)
        {
            return new GraphFilter($"_fulltext: {{ match: \"{EscapeGraphQL(value)}\" }}");
        }

        public GraphFilter Contains(string value)
        {
            return new GraphFilter($"_fulltext: {{ contains: \"{EscapeGraphQL(value)}\" }}");
        }

        public GraphFilter Fuzzy(string value, bool fuzzy = true)
        {
            return new GraphFilter($"_fulltext: {{ match: \"{EscapeGraphQL(value)}\", fuzzy: {fuzzy.ToString().ToLowerInvariant()} }}");
        }

        public GraphFilter Boost(string value, int boost = 10)
        {
            return new GraphFilter($"_fulltext: {{ match: \"{EscapeGraphQL(value)}\", boost: {boost} }}");
        }

        private static string EscapeGraphQL(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}

using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Represents a composable GraphQL filter condition for use in content queries.
    /// Filters are created via <see cref="GraphFilterBuilder"/> or composed using
    /// <see cref="And"/>, <see cref="Or"/>, and <see cref="Not"/>.
    /// </summary>
    public class GraphFilter
    {
        private readonly string _graphql;

        internal GraphFilter(string graphql)
        {
            _graphql = graphql;
        }

        /// <summary>
        /// Returns the GraphQL filter expression string.
        /// </summary>
        public override string ToString() => _graphql;

        /// <summary>
        /// Combines multiple filters with AND logic. All conditions must match.
        /// </summary>
        /// <param name="filters">The filters to combine.</param>
        /// <returns>A new <see cref="GraphFilter"/> representing the AND combination.</returns>
        public static GraphFilter And(params GraphFilter[] filters)
        {
            var parts = string.Join(", ", filters.Select(f => $"{{ {f} }}"));
            return new GraphFilter($"_and: [{parts}]");
        }

        /// <summary>
        /// Combines multiple filters with OR logic. At least one condition must match.
        /// </summary>
        /// <param name="filters">The filters to combine.</param>
        /// <returns>A new <see cref="GraphFilter"/> representing the OR combination.</returns>
        public static GraphFilter Or(params GraphFilter[] filters)
        {
            var parts = string.Join(", ", filters.Select(f => $"{{ {f} }}"));
            return new GraphFilter($"_or: [{parts}]");
        }

        /// <summary>
        /// Negates a filter. The condition must not match.
        /// </summary>
        /// <param name="filter">The filter to negate.</param>
        /// <returns>A new <see cref="GraphFilter"/> representing the negated condition.</returns>
        public static GraphFilter Not(GraphFilter filter)
        {
            return new GraphFilter($"_not: {{ {filter} }}");
        }
    }

    /// <summary>
    /// Entry point for building filter expressions in <c>Where()</c> clauses.
    /// Provides access to field, metadata, and full-text filters as well as boolean composition.
    /// </summary>
    public class GraphFilterBuilder
    {
        /// <summary>
        /// Creates a filter for a named content field.
        /// </summary>
        /// <param name="fieldName">The field name or dotted path (e.g. "title" or "author.name").</param>
        /// <returns>A <see cref="GraphFieldFilter"/> for specifying comparison operators.</returns>
        public GraphFieldFilter Field(string fieldName) => new GraphFieldFilter(fieldName);

        /// <summary>
        /// Provides access to metadata sub-filters such as key, URL, status, types, and locale.
        /// </summary>
        public GraphMetadataFilter Metadata => new GraphMetadataFilter();

        /// <summary>
        /// Combines multiple filters with AND logic.
        /// </summary>
        /// <param name="filters">The filters to combine.</param>
        /// <returns>A new <see cref="GraphFilter"/> representing the AND combination.</returns>
        public GraphFilter And(params GraphFilter[] filters) => GraphFilter.And(filters);

        /// <summary>
        /// Combines multiple filters with OR logic.
        /// </summary>
        /// <param name="filters">The filters to combine.</param>
        /// <returns>A new <see cref="GraphFilter"/> representing the OR combination.</returns>
        public GraphFilter Or(params GraphFilter[] filters) => GraphFilter.Or(filters);

        /// <summary>
        /// Negates a filter.
        /// </summary>
        /// <param name="filter">The filter to negate.</param>
        /// <returns>A new <see cref="GraphFilter"/> representing the negated condition.</returns>
        public GraphFilter Not(GraphFilter filter) => GraphFilter.Not(filter);

        /// <summary>
        /// Provides access to full-text search filters (match, contains, fuzzy, boost).
        /// </summary>
        public GraphFulltextFilter Fulltext => new GraphFulltextFilter();
    }

    /// <summary>
    /// Provides comparison operators for a specific content field. Created via
    /// <see cref="GraphFilterBuilder.Field"/> or one of the metadata filter accessors.
    /// </summary>
    public class GraphFieldFilter
    {
        private readonly string _fieldPath;

        internal GraphFieldFilter(string fieldPath)
        {
            _fieldPath = fieldPath;
        }

        /// <summary>Filters for values equal to the specified string.</summary>
        /// <param name="value">The value to match.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the equality condition.</returns>
        public GraphFilter Eq(string value) => Wrap("eq", FormatString(value));

        /// <summary>Filters for values equal to the specified integer.</summary>
        /// <param name="value">The value to match.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the equality condition.</returns>
        public GraphFilter Eq(int value) => Wrap("eq", value.ToString());

        /// <summary>Filters for values equal to the specified boolean.</summary>
        /// <param name="value">The value to match.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the equality condition.</returns>
        public GraphFilter Eq(bool value) => Wrap("eq", value.ToString().ToLowerInvariant());

        /// <summary>Filters for values equal to the specified date/time (ISO 8601 format).</summary>
        /// <param name="value">The value to match.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the equality condition.</returns>
        public GraphFilter Eq(DateTime value) => Wrap("eq", FormatString(value.ToString("O")));

        /// <summary>Filters for values not equal to the specified string.</summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the inequality condition.</returns>
        public GraphFilter NotEq(string value) => Wrap("notEq", FormatString(value));

        /// <summary>Filters for values not equal to the specified integer.</summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the inequality condition.</returns>
        public GraphFilter NotEq(int value) => Wrap("notEq", value.ToString());

        /// <summary>Filters for values matching a pattern with wildcards.</summary>
        /// <param name="value">The pattern to match (supports <c>*</c> and <c>?</c> wildcards).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the pattern match condition.</returns>
        public GraphFilter Like(string value) => Wrap("like", FormatString(value));

        /// <summary>Filters for values that start with the specified string.</summary>
        /// <param name="value">The prefix to match.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the starts-with condition.</returns>
        public GraphFilter StartsWith(string value) => Wrap("startsWith", FormatString(value));

        /// <summary>Filters for values that end with the specified string.</summary>
        /// <param name="value">The suffix to match.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the ends-with condition.</returns>
        public GraphFilter EndsWith(string value) => Wrap("endsWith", FormatString(value));

        /// <summary>Filters for values that contain the specified string.</summary>
        /// <param name="value">The substring to match.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the contains condition.</returns>
        public GraphFilter Contains(string value) => Wrap("contains", FormatString(value));

        /// <summary>Tests whether the field has a value (exists) or is null (does not exist).</summary>
        /// <param name="exists"><c>true</c> to match fields that have a value; <c>false</c> for null fields.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the existence condition.</returns>
        public GraphFilter Exists(bool exists) => Wrap("exist", exists.ToString().ToLowerInvariant());

        /// <summary>Filters for values that match any of the specified strings.</summary>
        /// <param name="values">The allowed values.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the membership condition.</returns>
        public GraphFilter In(params string[] values)
        {
            var formatted = string.Join(", ", values.Select(v => FormatString(v)));
            return Wrap("in", $"[{formatted}]");
        }

        /// <summary>Filters for values that match any of the specified integers.</summary>
        /// <param name="values">The allowed values.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the membership condition.</returns>
        public GraphFilter In(params int[] values)
        {
            var formatted = string.Join(", ", values);
            return Wrap("in", $"[{formatted}]");
        }

        /// <summary>Filters for values that do not match any of the specified strings.</summary>
        /// <param name="values">The excluded values.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the exclusion condition.</returns>
        public GraphFilter NotIn(params string[] values)
        {
            var formatted = string.Join(", ", values.Select(v => FormatString(v)));
            return Wrap("notIn", $"[{formatted}]");
        }

        /// <summary>Filters for integer values greater than the specified value.</summary>
        /// <param name="value">The lower bound (exclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
        public GraphFilter Gt(int value) => Wrap("gt", value.ToString());

        /// <summary>Filters for integer values greater than or equal to the specified value.</summary>
        /// <param name="value">The lower bound (inclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
        public GraphFilter Gte(int value) => Wrap("gte", value.ToString());

        /// <summary>Filters for integer values less than the specified value.</summary>
        /// <param name="value">The upper bound (exclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
        public GraphFilter Lt(int value) => Wrap("lt", value.ToString());

        /// <summary>Filters for integer values less than or equal to the specified value.</summary>
        /// <param name="value">The upper bound (inclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
        public GraphFilter Lte(int value) => Wrap("lte", value.ToString());

        /// <summary>Filters for date/time values greater than the specified value.</summary>
        /// <param name="value">The lower bound (exclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
        public GraphFilter Gt(DateTime value) => Wrap("gt", FormatString(value.ToString("O")));

        /// <summary>Filters for date/time values greater than or equal to the specified value.</summary>
        /// <param name="value">The lower bound (inclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
        public GraphFilter Gte(DateTime value) => Wrap("gte", FormatString(value.ToString("O")));

        /// <summary>Filters for date/time values less than the specified value.</summary>
        /// <param name="value">The upper bound (exclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
        public GraphFilter Lt(DateTime value) => Wrap("lt", FormatString(value.ToString("O")));

        /// <summary>Filters for date/time values less than or equal to the specified value.</summary>
        /// <param name="value">The upper bound (inclusive).</param>
        /// <returns>A <see cref="GraphFilter"/> representing the comparison.</returns>
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

    /// <summary>
    /// Provides access to metadata sub-filters for content queries.
    /// Accessed via <see cref="GraphFilterBuilder.Metadata"/>.
    /// </summary>
    public class GraphMetadataFilter
    {
        /// <summary>Filter by the content key (<c>_metadata.key</c>).</summary>
        public GraphFieldFilter Key => new GraphFieldFilter("_metadata.key");

        /// <summary>Access URL sub-filters (<c>_metadata.url.*</c>).</summary>
        public GraphMetadataUrlFilter Url => new GraphMetadataUrlFilter();

        /// <summary>Filter by content status (<c>_metadata.status</c>), e.g. "Published" or "Draft".</summary>
        public GraphFieldFilter Status => new GraphFieldFilter("_metadata.status");

        /// <summary>Filter by content types (<c>_metadata.types</c>).</summary>
        public GraphFieldFilter Types => new GraphFieldFilter("_metadata.types");

        /// <summary>Filter by display name (<c>_metadata.displayName</c>).</summary>
        public GraphFieldFilter DisplayName => new GraphFieldFilter("_metadata.displayName");

        /// <summary>Filter by published date (<c>_metadata.published</c>).</summary>
        public GraphFieldFilter Published => new GraphFieldFilter("_metadata.published");

        /// <summary>Filter by locale (<c>_metadata.locale</c>).</summary>
        public GraphFieldFilter Locale => new GraphFieldFilter("_metadata.locale");
    }

    /// <summary>
    /// Provides access to URL-specific metadata sub-filters.
    /// Accessed via <see cref="GraphMetadataFilter.Url"/>.
    /// </summary>
    public class GraphMetadataUrlFilter
    {
        /// <summary>Filter by the default URL path (<c>_metadata.url.default</c>).</summary>
        public GraphFieldFilter Default => new GraphFieldFilter("_metadata.url.default");

        /// <summary>Filter by the hierarchical URL path (<c>_metadata.url.hierarchical</c>).</summary>
        public GraphFieldFilter Hierarchical => new GraphFieldFilter("_metadata.url.hierarchical");

        /// <summary>Filter by the base URL (<c>_metadata.url.base</c>).</summary>
        public GraphFieldFilter Base => new GraphFieldFilter("_metadata.url.base");

        /// <summary>Filter by the internal URL (<c>_metadata.url.internal</c>).</summary>
        public GraphFieldFilter Internal => new GraphFieldFilter("_metadata.url.internal");
    }

    /// <summary>
    /// Provides full-text search filter operations for use in content queries.
    /// Accessed via <see cref="GraphFilterBuilder.Fulltext"/>.
    /// </summary>
    public class GraphFulltextFilter
    {
        /// <summary>
        /// Creates an exact full-text match filter.
        /// </summary>
        /// <param name="value">The text to search for.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the full-text match condition.</returns>
        public GraphFilter Match(string value)
        {
            return new GraphFilter($"_fulltext: {{ match: \"{EscapeGraphQL(value)}\" }}");
        }

        /// <summary>
        /// Creates a full-text contains filter.
        /// </summary>
        /// <param name="value">The text to search for.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the full-text contains condition.</returns>
        public GraphFilter Contains(string value)
        {
            return new GraphFilter($"_fulltext: {{ contains: \"{EscapeGraphQL(value)}\" }}");
        }

        /// <summary>
        /// Creates a full-text match filter with fuzzy matching that tolerates typos.
        /// </summary>
        /// <param name="value">The text to search for.</param>
        /// <param name="fuzzy">Whether to enable fuzzy matching. Defaults to <c>true</c>.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the fuzzy match condition.</returns>
        public GraphFilter Fuzzy(string value, bool fuzzy = true)
        {
            return new GraphFilter($"_fulltext: {{ match: \"{EscapeGraphQL(value)}\", fuzzy: {fuzzy.ToString().ToLowerInvariant()} }}");
        }

        /// <summary>
        /// Creates a full-text match filter with a relevance boost factor.
        /// </summary>
        /// <param name="value">The text to search for.</param>
        /// <param name="boost">The boost weight. Defaults to 10.</param>
        /// <returns>A <see cref="GraphFilter"/> representing the boosted match condition.</returns>
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

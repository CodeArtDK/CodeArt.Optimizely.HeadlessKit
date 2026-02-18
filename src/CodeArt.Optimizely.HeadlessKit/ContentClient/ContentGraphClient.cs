using CodeArt.Optimizely.HeadlessKit.Core;
using CodeArt.Optimizely.HeadlessKit.Core.Models;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Main client for querying content from Optimizely Graph via GraphQL.
    /// Provides methods to load content by path, key, or version, as well as
    /// a fluent query builder entry point and raw query execution.
    /// </summary>
    /// <example>
    /// <code>
    /// // Load a page by URL path
    /// var page = await client.GetContentByPath&lt;StandardPage&gt;("/en/about-us");
    ///
    /// // Use the fluent query builder
    /// var results = await client.Query&lt;StandardPage&gt;()
    ///     .Where(f => f.Metadata.Status.Eq("Published"))
    ///     .Take(10)
    ///     .ToListAsync();
    /// </code>
    /// </example>
    public class ContentGraphClient
    {
        private readonly IGraphQueryProvider _graphQueryProvider;
        private readonly ContentGraphOptions _options;
        private readonly IContentTypeRegistry? _contentTypeRegistry;

        private GraphQLHttpClient _client = new GraphQLHttpClient("https://cg.optimizely.com/content/v2?auth=placeholder", new SystemTextJsonSerializer());

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentGraphClient"/> class.
        /// </summary>
        /// <param name="graphQueryProvider">Provider for pre-built GraphQL queries.</param>
        /// <param name="options">Optimizely Graph connection options including endpoint and single key.</param>
        /// <param name="contentTypeRegistry">Optional registry for resolving content types at runtime.</param>
        public ContentGraphClient(IGraphQueryProvider graphQueryProvider, IOptions<ContentGraphOptions> options, IContentTypeRegistry? contentTypeRegistry = null)
        {
            _graphQueryProvider = graphQueryProvider ?? throw new ArgumentNullException(nameof(graphQueryProvider));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _contentTypeRegistry = contentTypeRegistry;

            _client = new GraphQLHttpClient(string.Format("{0}?auth={1}", _options.Endpoint, _options.SingleKey), new SystemTextJsonSerializer());
        }

        private bool DebugLogging => _options.DebugLogging;

        private static readonly JsonSerializerOptions _debugJsonOptions = new() { WriteIndented = true };

        private void LogQuery(string operationName, string query, object? variables)
        {
            if (!DebugLogging) return;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[Graph Query] {operationName}");
            Console.ResetColor();
            Console.WriteLine(query);
            if (variables != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"[Variables] {JsonSerializer.Serialize(variables, _debugJsonOptions)}");
                Console.ResetColor();
            }
        }

        private void LogResponse(string operationName, JsonElement data, GraphQLError[]? errors)
        {
            if (!DebugLogging) return;
            if (errors != null && errors.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Graph Errors] {operationName}");
                foreach (var err in errors)
                    Console.WriteLine($"  - {err.Message}");
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Graph Response] {operationName}");
            Console.ResetColor();
            if (data.ValueKind == JsonValueKind.Undefined || data.ValueKind == JsonValueKind.Null)
                Console.WriteLine("  (no data)");
            else
                Console.WriteLine(JsonSerializer.Serialize(data, _debugJsonOptions));
        }

        /// <summary>
        /// Returns a fluent <see cref="GraphQueryBuilder{T}"/> for building custom typed GraphQL queries.
        /// </summary>
        /// <typeparam name="T">The content model type to query for.</typeparam>
        /// <returns>A new <see cref="GraphQueryBuilder{T}"/> pre-configured with this client and the content type registry.</returns>
        public GraphQueryBuilder<T> Query<T>() where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>(this, _contentTypeRegistry!);
        }

        /// <summary>
        /// Loads a single content item by its URL path.
        /// </summary>
        /// <typeparam name="TContentModel">The content model type to deserialize into.</typeparam>
        /// <param name="path">The URL path of the content item (e.g. "/en/about-us").</param>
        /// <param name="locale">Optional locale filter. Defaults to all locales if not specified.</param>
        /// <returns>The content item, or <c>null</c> if not found.</returns>
        public async Task<TContentModel?> GetContentByPath<TContentModel>(string path, string[]? locale = null) where TContentModel : class, IGraphContent
        {

            var content = await LoadContentFromPath<TContentModel>(path, locale == null ? new string[] { "ALL" } : locale);

            return content;
        }

        /// <summary>
        /// Extracts and deserializes the content from an _Experience { item { _json } } response.
        /// Resolves the concrete type from __typename before deserializing.
        /// </summary>
        private TContentModel? DeserializeExperienceJson<TContentModel>(JsonElement data) where TContentModel : class, IGraphContent
        {
            if (data.ValueKind == JsonValueKind.Undefined || data.ValueKind == JsonValueKind.Null)
                return null;

            var experienceEl = data.Get("_Experience");
            if (!experienceEl.HasValue)
                return null;

            var itemEl = experienceEl.Value.Get("item");
            if (!itemEl.HasValue || itemEl.Value.ValueKind == JsonValueKind.Null)
                return null;

            var jsonEl = itemEl.Value.Get("_json");
            if (!jsonEl.HasValue || jsonEl.Value.ValueKind == JsonValueKind.Null)
                return null;

            var options = GenerateDeserializationOptions();

            // Resolve the concrete type from __typename (which may not be the first property)
            if (jsonEl.Value.TryGetProperty("__typename", out var typeNameEl) && typeNameEl.ValueKind == JsonValueKind.String)
            {
                var typeName = typeNameEl.GetString();
                if (typeName != null)
                {
                    var resolvedType = GraphContentJsonConverter.ResolveType(typeName);
                    if (resolvedType != null && typeof(TContentModel).IsAssignableFrom(resolvedType))
                    {
                        return (TContentModel?)jsonEl.Value.Deserialize(resolvedType, options);
                    }
                }
            }

            // Fallback to direct deserialization
            return jsonEl.Value.Deserialize<TContentModel>(options);
        }

        internal JsonSerializerOptions GenerateDeserializationOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new GraphContentJsonConverter()
                }
            };
        }


        private async Task<TContentModel?> LoadContentFromPath<TContentModel>(string path, string[] localeList) where TContentModel : class, IGraphContent
        {
            const string query = @"
query MainContentQuery($path: String!, $locale: [Locales]) {
  _Experience(
    where: { _metadata: { url: { default: { eq: $path } } } }
    locale: $locale
  ) {
    item {
      _json
    }
  }
}";
            var variables = new { path, locale = localeList };
            var req = new GraphQLRequest
            {
                Query = query,
                OperationName = "MainContentQuery",
                Variables = variables
            };

            LogQuery("MainContentQuery", query, variables);
            var r1 = await _client.SendQueryAsync<JsonElement>(req);
            LogResponse("MainContentQuery", r1.Data, r1.Errors);

            return DeserializeExperienceJson<TContentModel>(r1.Data);
        }
        /// <summary>
        /// Loads a single content item by its content key.
        /// </summary>
        /// <typeparam name="TContentModel">The content model type to deserialize into.</typeparam>
        /// <param name="key">The unique content key.</param>
        /// <param name="locale">Optional locale filter. Defaults to all locales if not specified.</param>
        /// <returns>The content item, or <c>null</c> if not found.</returns>
        public async Task<TContentModel?> GetContentByKey<TContentModel>(string key, string[]? locale = null) where TContentModel : class, IGraphContent
        {
            const string query = @"
query GetContentByKey($key: String!, $locale: [Locales]) {
  _Experience(
    where: { _metadata: { key: { eq: $key } } }
    locale: $locale
  ) {
    item {
      _json
    }
  }
}";
            var variables = new { key, locale = locale ?? new string[] { "ALL" } };
            var req = new GraphQLRequest
            {
                Query = query,
                OperationName = "GetContentByKey",
                Variables = variables
            };

            LogQuery("GetContentByKey", query, variables);
            var r1 = await _client.SendQueryAsync<JsonElement>(req);
            LogResponse("GetContentByKey", r1.Data, r1.Errors);

            return DeserializeExperienceJson<TContentModel>(r1.Data);
        }

        /// <summary>
        /// Loads a specific content version for CMS preview. Uses Bearer token authentication
        /// to access draft or versioned content not yet published.
        /// </summary>
        /// <typeparam name="TContentModel">The content model type to deserialize into.</typeparam>
        /// <param name="key">The unique content key.</param>
        /// <param name="version">The content version identifier.</param>
        /// <param name="previewToken">The CMS preview bearer token for authentication.</param>
        /// <param name="locale">Optional locale filter. Defaults to all locales if not specified.</param>
        /// <returns>The content item at the specified version, or <c>null</c> if not found.</returns>
        public async Task<TContentModel?> GetPreviewContentByKey<TContentModel>(string key, string version, string previewToken, string[]? locale = null) where TContentModel : class, IGraphContent
        {
            const string query = @"
query GetContentByKeyAndVersion($key: String!, $version: String, $locale: [Locales]) {
  _Experience(
    where: { _metadata: { key: { eq: $key }, version: { eq: $version } } }
    locale: $locale
  ) {
    item {
      _json
    }
  }
}";
            var variables = new { key, version, locale = locale ?? new string[] { "ALL" } };
            var req = new GraphQLHttpRequestWithAuthSupport
            {
                Query = query,
                OperationName = "GetContentByKeyAndVersion",
                Variables = variables,
                Authentication = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", previewToken)
            };

            LogQuery("GetContentByKeyAndVersion", query, variables);
            var r1 = await _client.SendQueryAsync<JsonElement>(req);
            LogResponse("GetContentByKeyAndVersion", r1.Data, r1.Errors);

            return DeserializeExperienceJson<TContentModel>(r1.Data);
        }

        private const string GetChildrenQuery = @"
query GetChildrenByKey($key: String!, $locale: [Locales]) {
  _Content(
    where: { _metadata: { key: { eq: $key } } }
    locale: $locale
  ) {
    items {
      _link {
        _Content {
          items {
            __typename
            _metadata { key displayName types locale url { default hierarchical } published status created lastModified version }
          }
        }
      }
    }
  }
}";

        /// <summary>
        /// Loads child content items for a given parent content key.
        /// </summary>
        /// <typeparam name="TContentModel">The content model type to deserialize children into.</typeparam>
        /// <param name="parentkey">The content key of the parent item.</param>
        /// <param name="locale">Optional locale filter.</param>
        /// <returns>A list of child content items, or <c>null</c> if the parent is not found.</returns>
        public async Task<List<TContentModel>?> GetChildren<TContentModel>(string parentkey, string[]? locale = null) where TContentModel : class, IGraphContent
        {
            var variables = new { key = parentkey, locale };
            var req = new GraphQLRequest
            {
                Query = GetChildrenQuery,
                OperationName = "GetChildrenByKey",
                Variables = variables
            };

            LogQuery("GetChildrenByKey", GetChildrenQuery, variables);
            var r1 = await _client.SendQueryAsync<JsonElement>(req);
            LogResponse("GetChildrenByKey", r1.Data, r1.Errors);

            var contentEl = r1.Data.Get("_Content");
            if (!contentEl.HasValue) return null;
            var itemsEl = contentEl.Value.Get("items");
            if (!itemsEl.HasValue || itemsEl.Value.GetArrayLength() == 0) return null;
            var linkEl = itemsEl.Value[0].Get("_link");
            if (!linkEl.HasValue) return null;
            var resp = linkEl.Value.Deserialize<GraphResponse<TContentModel>>(GenerateDeserializationOptions());

            return resp?.Content?.Items?.OfType<TContentModel>().ToList();
        }

        /// <summary>
        /// Executes a raw GraphQL query string and deserializes the response.
        /// Uses the type name of <typeparamref name="T"/> as the response key.
        /// </summary>
        /// <typeparam name="T">The content model type to deserialize results into.</typeparam>
        /// <param name="graphqlQuery">The raw GraphQL query string.</param>
        /// <returns>A <see cref="GraphQueryResult{T}"/> containing the matched items, total count, and pagination cursor.</returns>
        /// <exception cref="GraphQueryException">Thrown when the GraphQL query returns errors.</exception>
        public Task<GraphQueryResult<T>> ExecuteQueryAsync<T>(string graphqlQuery) where T : class, IGraphContent, new()
            => ExecuteQueryAsync<T>(graphqlQuery, typeof(T).Name);

        /// <summary>
        /// Executes a raw GraphQL query string with a custom response key for locating
        /// the result data in the GraphQL response.
        /// </summary>
        /// <typeparam name="T">The content model type to deserialize results into.</typeparam>
        /// <param name="graphqlQuery">The raw GraphQL query string.</param>
        /// <param name="responseKey">The key in the GraphQL response that contains the result data (e.g. type name or alias).</param>
        /// <returns>A <see cref="GraphQueryResult{T}"/> containing the matched items, total count, and pagination cursor.</returns>
        /// <exception cref="GraphQueryException">Thrown when the GraphQL query returns errors.</exception>
        public async Task<GraphQueryResult<T>> ExecuteQueryAsync<T>(string graphqlQuery, string responseKey) where T : class, IGraphContent, new()
        {
            var typeName = responseKey;

            var req = new GraphQLRequest
            {
                Query = graphqlQuery
            };

            LogQuery($"ExecuteQuery<{typeName}>", graphqlQuery, null);
            var r1 = await _client.SendQueryAsync<JsonElement>(req);
            LogResponse($"ExecuteQuery<{typeName}>", r1.Data, r1.Errors);

            // Check for GraphQL errors
            if (r1.Errors != null && r1.Errors.Length > 0)
            {
                var errorMessages = r1.Errors.Select(e => e.Message).ToList();
                throw new GraphQueryException(errorMessages);
            }

            var deserializationOptions = GenerateDeserializationOptions();

            var result = new GraphQueryResult<T>();

            // The response structure is { TypeName: { items: [...], total: N, cursor: "..." } }
            var typeElement = r1.Data.Get(typeName);
            if (typeElement.HasValue)
            {
                var itemsElement = typeElement.Value.Get("items");
                if (itemsElement.HasValue)
                {
                    result.Items = itemsElement.Value.Deserialize<List<T>>(deserializationOptions) ?? new List<T>();
                }

                var totalElement = typeElement.Value.Get("total");
                if (totalElement.HasValue && totalElement.Value.ValueKind == JsonValueKind.Number)
                {
                    result.Total = totalElement.Value.GetInt32();
                }

                var cursorElement = typeElement.Value.Get("cursor");
                if (cursorElement.HasValue && cursorElement.Value.ValueKind == JsonValueKind.String)
                {
                    result.Cursor = cursorElement.Value.GetString();
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a query that returns items with <c>_json</c> fields for polymorphic deserialization.
        /// Each item's <c>_json</c> is deserialized using the <c>__typename</c> field to resolve the concrete type.
        /// Useful for union type queries such as <c>_Page</c>.
        /// </summary>
        /// <typeparam name="T">The base content model type to deserialize results into.</typeparam>
        /// <param name="graphqlQuery">The raw GraphQL query string.</param>
        /// <param name="responseKey">The key in the GraphQL response that contains the result data.</param>
        /// <returns>A <see cref="GraphQueryResult{T}"/> containing the matched items, total count, and pagination cursor.</returns>
        /// <exception cref="GraphQueryException">Thrown when the GraphQL query returns errors.</exception>
        public async Task<GraphQueryResult<T>> ExecuteJsonItemsQueryAsync<T>(string graphqlQuery, string responseKey) where T : class, IGraphContent, new()
        {
            var req = new GraphQLRequest
            {
                Query = graphqlQuery
            };

            LogQuery($"ExecuteJsonQuery<{responseKey}>", graphqlQuery, null);
            var r1 = await _client.SendQueryAsync<JsonElement>(req);
            LogResponse($"ExecuteJsonQuery<{responseKey}>", r1.Data, r1.Errors);

            if (r1.Errors != null && r1.Errors.Length > 0)
            {
                var errorMessages = r1.Errors.Select(e => e.Message).ToList();
                throw new GraphQueryException(errorMessages);
            }

            var options = GenerateDeserializationOptions();
            var result = new GraphQueryResult<T>();

            var typeElement = r1.Data.Get(responseKey);
            if (typeElement.HasValue)
            {
                var itemsElement = typeElement.Value.Get("items");
                if (itemsElement.HasValue && itemsElement.Value.ValueKind == JsonValueKind.Array)
                {
                    var items = new List<T>();
                    foreach (var itemEl in itemsElement.Value.EnumerateArray())
                    {
                        var jsonEl = itemEl.Get("_json");
                        if (!jsonEl.HasValue || jsonEl.Value.ValueKind == JsonValueKind.Null)
                            continue;

                        T? deserialized = null;
                        if (jsonEl.Value.TryGetProperty("__typename", out var typeNameEl) && typeNameEl.ValueKind == JsonValueKind.String)
                        {
                            var resolvedType = GraphContentJsonConverter.ResolveType(typeNameEl.GetString()!);
                            if (resolvedType != null && typeof(T).IsAssignableFrom(resolvedType))
                                deserialized = (T?)jsonEl.Value.Deserialize(resolvedType, options);
                        }
                        deserialized ??= jsonEl.Value.Deserialize<T>(options);
                        if (deserialized != null)
                            items.Add(deserialized);
                    }
                    result.Items = items;
                }

                var totalElement = typeElement.Value.Get("total");
                if (totalElement.HasValue && totalElement.Value.ValueKind == JsonValueKind.Number)
                    result.Total = totalElement.Value.GetInt32();

                var cursorElement = typeElement.Value.Get("cursor");
                if (cursorElement.HasValue && cursorElement.Value.ValueKind == JsonValueKind.String)
                    result.Cursor = cursorElement.Value.GetString();
            }

            return result;
        }
    }

    /// <summary>
    /// Extension methods for safe navigation of <see cref="JsonElement"/> values.
    /// </summary>
    public static partial class JsonExtensions
    {
        /// <summary>
        /// Gets a child property by name, returning <c>null</c> if the element is null, undefined, or the property does not exist.
        /// </summary>
        /// <param name="element">The JSON element to read from.</param>
        /// <param name="name">The property name to look up.</param>
        /// <returns>The child <see cref="JsonElement"/>, or <c>null</c> if not found.</returns>
        public static JsonElement? Get(this JsonElement element, string name) =>
            element.ValueKind != JsonValueKind.Null && element.ValueKind != JsonValueKind.Undefined && element.TryGetProperty(name, out var value)
                ? value : (JsonElement?)null;

        /// <summary>
        /// Gets an array element by index, returning <c>null</c> if the element is null, undefined, or the index is out of range.
        /// </summary>
        /// <param name="element">The JSON array element to read from.</param>
        /// <param name="index">The zero-based index of the array element.</param>
        /// <returns>The array element at the specified index, or <c>null</c> if not found.</returns>
        public static JsonElement? Get(this JsonElement element, int index)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
                return null;
            // Throw if index < 0
            return index < element.GetArrayLength() ? element[index] : null;
        }
    }
}

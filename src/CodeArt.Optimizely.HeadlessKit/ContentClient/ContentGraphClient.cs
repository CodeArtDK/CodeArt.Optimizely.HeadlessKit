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
    public class ContentGraphClient
    {
        private readonly IGraphQueryProvider _graphQueryProvider;
        private readonly ContentGraphOptions _options;
        private readonly IContentTypeRegistry? _contentTypeRegistry;

        private GraphQLHttpClient _client = new GraphQLHttpClient("https://cg.optimizely.com/content/v2?auth=placeholder", new SystemTextJsonSerializer());

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

        // Fluent query builder entry point
        public GraphQueryBuilder<T> Query<T>() where T : class, IGraphContent, new()
        {
            return new GraphQueryBuilder<T>(this, _contentTypeRegistry!);
        }

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

        // Execute a raw GraphQL query string and deserialize the response
        public Task<GraphQueryResult<T>> ExecuteQueryAsync<T>(string graphqlQuery) where T : class, IGraphContent, new()
            => ExecuteQueryAsync<T>(graphqlQuery, typeof(T).Name);

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
        /// Execute a query that returns items with _json fields (for union types like _Page).
        /// Each item's _json is deserialized polymorphically using __typename.
        /// </summary>
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

    public static partial class JsonExtensions
    {
        public static JsonElement? Get(this JsonElement element, string name) =>
            element.ValueKind != JsonValueKind.Null && element.ValueKind != JsonValueKind.Undefined && element.TryGetProperty(name, out var value)
                ? value : (JsonElement?)null;

        public static JsonElement? Get(this JsonElement element, int index)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
                return null;
            // Throw if index < 0
            return index < element.GetArrayLength() ? element[index] : null;
        }
    }
}

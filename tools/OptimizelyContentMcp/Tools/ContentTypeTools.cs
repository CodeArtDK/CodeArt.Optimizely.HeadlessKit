using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class ContentTypeTools
{
    [McpServerTool(Name = "list_content_types")]
    [Description("List all content types in the CMS. Returns paginated results. Use 'sources' to filter by source (e.g. 'code', 'ui').")]
    public static async Task<string> ListContentTypes(
        OptimizelyApiClient client,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        [Description("Filter by source, e.g. 'code' or 'ui'")] string? sources = null,
        CancellationToken ct = default)
    {
        var query = $"contenttypes?pageIndex={pageIndex}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(sources))
            query += $"&sources={Uri.EscapeDataString(sources)}";
        return await client.GetAsync(query, ct);
    }

    [McpServerTool(Name = "get_content_type")]
    [Description("Get a single content type by its key. Returns the full content type definition including properties.")]
    public static async Task<string> GetContentType(
        OptimizelyApiClient client,
        [Description("The content type key (e.g. 'BlogPage')")] string key,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"contenttypes/{Uri.EscapeDataString(key)}", ct);
    }

    [McpServerTool(Name = "create_content_type")]
    [Description("Create a new content type. The body must be a JSON object with at least 'key', 'displayName', and 'baseType'. Example baseType values: 'page', 'block', 'media', 'component', 'experience', 'section', 'element'.")]
    public static async Task<string> CreateContentType(
        OptimizelyApiClient client,
        [Description("JSON body for the new content type")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync("contenttypes", body, ct);
    }

    [McpServerTool(Name = "update_content_type")]
    [Description("Update an existing content type using merge-patch. Only include the fields you want to change. To add/update properties, include them in the 'properties' object.")]
    public static async Task<string> UpdateContentType(
        OptimizelyApiClient client,
        [Description("The content type key")] string key,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"contenttypes/{Uri.EscapeDataString(key)}", body, ct);
    }

    [McpServerTool(Name = "delete_content_type")]
    [Description("Delete a content type by its key. WARNING: This is a destructive operation.")]
    public static async Task<string> DeleteContentType(
        OptimizelyApiClient client,
        [Description("The content type key to delete")] string key,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"contenttypes/{Uri.EscapeDataString(key)}", ct);
    }
}

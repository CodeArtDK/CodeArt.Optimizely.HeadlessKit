using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class ContentTools
{
    [McpServerTool(Name = "get_content")]
    [Description("Get a content item by its key (GUID). Returns the content with all properties. Use 'locale' to get a specific language version.")]
    public static async Task<string> GetContent(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("Optional locale code, e.g. 'en' or 'sv'")] string? locale = null,
        CancellationToken ct = default)
    {
        var path = $"experimental/content/{Uri.EscapeDataString(key)}";
        if (!string.IsNullOrEmpty(locale))
            path += $"?locale={Uri.EscapeDataString(locale)}";
        return await client.GetAsync(path, ct);
    }

    [McpServerTool(Name = "get_content_path")]
    [Description("Get the URL path for a content item by its key. Returns the routing path used on the site.")]
    public static async Task<string> GetContentPath(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/content/{Uri.EscapeDataString(key)}/path", ct);
    }

    [McpServerTool(Name = "list_content_children")]
    [Description("List child content items under a parent content key. Returns paginated results.")]
    public static async Task<string> ListContentChildren(
        OptimizelyApiClient client,
        [Description("The parent content key (GUID)")] string key,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/content/{Uri.EscapeDataString(key)}/items?pageIndex={pageIndex}&pageSize={pageSize}", ct);
    }

    [McpServerTool(Name = "list_content_assets")]
    [Description("List asset items (media files) associated with a content item.")]
    public static async Task<string> ListContentAssets(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/content/{Uri.EscapeDataString(key)}/assets", ct);
    }

    [McpServerTool(Name = "list_content_versions")]
    [Description("List all versions of a content item. Returns paginated version history.")]
    public static async Task<string> ListContentVersions(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/content/{Uri.EscapeDataString(key)}/versions?pageIndex={pageIndex}&pageSize={pageSize}", ct);
    }

    [McpServerTool(Name = "query_content_versions")]
    [Description("Query versions of a content item with advanced filtering.")]
    public static async Task<string> QueryContentVersions(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/content/{Uri.EscapeDataString(key)}/versions/query", ct);
    }

    [McpServerTool(Name = "get_content_version")]
    [Description("Get a specific version of a content item by its version identifier.")]
    public static async Task<string> GetContentVersion(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("The version identifier")] string version,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/content/{Uri.EscapeDataString(key)}/versions/{Uri.EscapeDataString(version)}", ct);
    }

    [McpServerTool(Name = "create_content")]
    [Description("Create a new content item. The body must be a JSON object with at least 'contentType', 'displayName', and 'parentLink'. Properties go in a locale-keyed object under the content type key.")]
    public static async Task<string> CreateContent(
        OptimizelyApiClient client,
        [Description("JSON body for the new content item")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync("experimental/content", body, ct);
    }

    [McpServerTool(Name = "copy_content")]
    [Description("Copy a content item to a new location. The body should specify the destination parent.")]
    public static async Task<string> CopyContent(
        OptimizelyApiClient client,
        [Description("The content key (GUID) to copy")] string key,
        [Description("JSON body specifying copy options (e.g. destination parent)")] string body,
        CancellationToken ct = default)
    {
        return await client.PostActionAsync($"experimental/content/{Uri.EscapeDataString(key)}:copy", body, ct);
    }

    [McpServerTool(Name = "undelete_content")]
    [Description("Restore a previously deleted content item from the trash.")]
    public static async Task<string> UndeleteContent(
        OptimizelyApiClient client,
        [Description("The content key (GUID) to restore")] string key,
        CancellationToken ct = default)
    {
        return await client.PostActionAsync($"experimental/content/{Uri.EscapeDataString(key)}:undelete", null, ct);
    }

    [McpServerTool(Name = "create_content_version")]
    [Description("Create a new version (draft) of an existing content item. The body contains the updated property values.")]
    public static async Task<string> CreateContentVersion(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("JSON body for the new version")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync($"experimental/content/{Uri.EscapeDataString(key)}/versions", body, ct);
    }

    [McpServerTool(Name = "update_content")]
    [Description("Update a content item using merge-patch. Only include the fields you want to change.")]
    public static async Task<string> UpdateContent(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"experimental/content/{Uri.EscapeDataString(key)}", body, ct);
    }

    [McpServerTool(Name = "update_content_version")]
    [Description("Update a specific version of a content item using merge-patch.")]
    public static async Task<string> UpdateContentVersion(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("The version identifier")] string version,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"experimental/content/{Uri.EscapeDataString(key)}/versions/{Uri.EscapeDataString(version)}", body, ct);
    }

    [McpServerTool(Name = "delete_content")]
    [Description("Delete a content item by its key. WARNING: This moves the content to trash or permanently deletes it.")]
    public static async Task<string> DeleteContent(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"experimental/content/{Uri.EscapeDataString(key)}", ct);
    }

    [McpServerTool(Name = "delete_content_locale")]
    [Description("Delete all versions of a content item for a specific locale/language.")]
    public static async Task<string> DeleteContentLocale(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("The locale to delete, e.g. 'sv' or 'en'")] string locale,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"experimental/content/{Uri.EscapeDataString(key)}/versions?locale={Uri.EscapeDataString(locale)}", ct);
    }

    [McpServerTool(Name = "delete_content_version")]
    [Description("Delete a specific version of a content item.")]
    public static async Task<string> DeleteContentVersion(
        OptimizelyApiClient client,
        [Description("The content key (GUID)")] string key,
        [Description("The version identifier to delete")] string version,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"experimental/content/{Uri.EscapeDataString(key)}/versions/{Uri.EscapeDataString(version)}", ct);
    }
}

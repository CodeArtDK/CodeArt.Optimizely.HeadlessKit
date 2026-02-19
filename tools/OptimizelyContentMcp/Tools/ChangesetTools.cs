using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class ChangesetTools
{
    [McpServerTool(Name = "list_changesets")]
    [Description("List all changesets. Changesets group related content changes for batch publishing.")]
    public static async Task<string> ListChangesets(
        OptimizelyApiClient client,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/changesets?pageIndex={pageIndex}&pageSize={pageSize}", ct);
    }

    [McpServerTool(Name = "get_changeset")]
    [Description("Get a single changeset by its ID.")]
    public static async Task<string> GetChangeset(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/changesets/{Uri.EscapeDataString(id)}", ct);
    }

    [McpServerTool(Name = "create_changeset")]
    [Description("Create a new changeset. The body should contain at least a 'name' and optional 'description'.")]
    public static async Task<string> CreateChangeset(
        OptimizelyApiClient client,
        [Description("JSON body for the new changeset")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync("experimental/changesets", body, ct);
    }

    [McpServerTool(Name = "update_changeset")]
    [Description("Update an existing changeset using merge-patch. Only include the fields you want to change.")]
    public static async Task<string> UpdateChangeset(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"experimental/changesets/{Uri.EscapeDataString(id)}", body, ct);
    }

    [McpServerTool(Name = "delete_changeset")]
    [Description("Delete a changeset by its ID.")]
    public static async Task<string> DeleteChangeset(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"experimental/changesets/{Uri.EscapeDataString(id)}", ct);
    }

    [McpServerTool(Name = "list_changeset_items")]
    [Description("List all items in a changeset. Each item represents a content change included in the changeset.")]
    public static async Task<string> ListChangesetItems(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/changesets/{Uri.EscapeDataString(id)}/items?pageIndex={pageIndex}&pageSize={pageSize}", ct);
    }

    [McpServerTool(Name = "get_changeset_item")]
    [Description("Get a specific item from a changeset.")]
    public static async Task<string> GetChangesetItem(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        [Description("The changeset item ID")] string itemId,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/changesets/{Uri.EscapeDataString(id)}/items/{Uri.EscapeDataString(itemId)}", ct);
    }

    [McpServerTool(Name = "add_changeset_item")]
    [Description("Add a content item to a changeset. The body should reference the content key and version to include.")]
    public static async Task<string> AddChangesetItem(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        [Description("JSON body for the new changeset item")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync($"experimental/changesets/{Uri.EscapeDataString(id)}/items", body, ct);
    }

    [McpServerTool(Name = "update_changeset_item")]
    [Description("Update a changeset item using merge-patch.")]
    public static async Task<string> UpdateChangesetItem(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        [Description("The changeset item ID")] string itemId,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"experimental/changesets/{Uri.EscapeDataString(id)}/items/{Uri.EscapeDataString(itemId)}", body, ct);
    }

    [McpServerTool(Name = "delete_changeset_item")]
    [Description("Remove an item from a changeset.")]
    public static async Task<string> DeleteChangesetItem(
        OptimizelyApiClient client,
        [Description("The changeset ID")] string id,
        [Description("The changeset item ID")] string itemId,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"experimental/changesets/{Uri.EscapeDataString(id)}/items/{Uri.EscapeDataString(itemId)}", ct);
    }
}

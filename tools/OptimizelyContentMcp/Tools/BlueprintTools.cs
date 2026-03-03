using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class BlueprintTools
{
    [McpServerTool(Name = "list_blueprints")]
    [Description("List all content blueprints (templates). Blueprints are pre-filled content items that editors can use as starting points when creating new content.")]
    public static async Task<string> ListBlueprints(
        OptimizelyApiClient client,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/blueprints?pageIndex={pageIndex}&pageSize={pageSize}", ct);
    }

    [McpServerTool(Name = "get_blueprint")]
    [Description("Get a single blueprint by its key.")]
    public static async Task<string> GetBlueprint(
        OptimizelyApiClient client,
        [Description("The blueprint key")] string key,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/blueprints/{Uri.EscapeDataString(key)}", ct);
    }

    [McpServerTool(Name = "create_blueprint")]
    [Description("Create a new content blueprint. The body should contain the blueprint definition including content type and pre-filled property values.")]
    public static async Task<string> CreateBlueprint(
        OptimizelyApiClient client,
        [Description("JSON body for the new blueprint")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync("experimental/blueprints", body, ct);
    }

    [McpServerTool(Name = "update_blueprint")]
    [Description("Update an existing blueprint using merge-patch. Only include the fields you want to change.")]
    public static async Task<string> UpdateBlueprint(
        OptimizelyApiClient client,
        [Description("The blueprint key")] string key,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"experimental/blueprints/{Uri.EscapeDataString(key)}", body, ct);
    }

    [McpServerTool(Name = "delete_blueprint")]
    [Description("Delete a blueprint by its key.")]
    public static async Task<string> DeleteBlueprint(
        OptimizelyApiClient client,
        [Description("The blueprint key")] string key,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"experimental/blueprints/{Uri.EscapeDataString(key)}", ct);
    }
}

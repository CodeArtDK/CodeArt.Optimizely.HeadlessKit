using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class PropertyGroupTools
{
    [McpServerTool(Name = "list_property_groups")]
    [Description("List all property groups in the CMS. Property groups organize properties into tabs/sections in the editor UI.")]
    public static async Task<string> ListPropertyGroups(
        OptimizelyApiClient client,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"propertygroups?pageIndex={pageIndex}&pageSize={pageSize}", ct);
    }

    [McpServerTool(Name = "get_property_group")]
    [Description("Get a single property group by its ID.")]
    public static async Task<string> GetPropertyGroup(
        OptimizelyApiClient client,
        [Description("The property group ID")] string id,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"propertygroups/{Uri.EscapeDataString(id)}", ct);
    }

    [McpServerTool(Name = "create_property_group")]
    [Description("Create a new property group. The body must be a JSON object with at least 'key' and 'displayName'.")]
    public static async Task<string> CreatePropertyGroup(
        OptimizelyApiClient client,
        [Description("JSON body for the new property group")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync("propertygroups", body, ct);
    }

    [McpServerTool(Name = "update_property_group")]
    [Description("Update an existing property group using merge-patch. Only include the fields you want to change.")]
    public static async Task<string> UpdatePropertyGroup(
        OptimizelyApiClient client,
        [Description("The property group ID")] string id,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"propertygroups/{Uri.EscapeDataString(id)}", body, ct);
    }

    [McpServerTool(Name = "delete_property_group")]
    [Description("Delete a property group by its ID.")]
    public static async Task<string> DeletePropertyGroup(
        OptimizelyApiClient client,
        [Description("The property group ID")] string id,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"propertygroups/{Uri.EscapeDataString(id)}", ct);
    }
}

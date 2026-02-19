using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class DisplayTemplateTools
{
    [McpServerTool(Name = "list_display_templates")]
    [Description("List all display templates in the CMS. Returns paginated results.")]
    public static async Task<string> ListDisplayTemplates(
        OptimizelyApiClient client,
        [Description("Page index (0-based), default 0")] int pageIndex = 0,
        [Description("Page size, default 50")] int pageSize = 50,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"displaytemplates?pageIndex={pageIndex}&pageSize={pageSize}", ct);
    }

    [McpServerTool(Name = "get_display_template")]
    [Description("Get a single display template by its ID.")]
    public static async Task<string> GetDisplayTemplate(
        OptimizelyApiClient client,
        [Description("The display template ID")] string id,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"displaytemplates/{Uri.EscapeDataString(id)}", ct);
    }

    [McpServerTool(Name = "create_display_template")]
    [Description("Create a new display template. The body must be a JSON object with 'key', 'contentType', and optional 'settings' array.")]
    public static async Task<string> CreateDisplayTemplate(
        OptimizelyApiClient client,
        [Description("JSON body for the new display template")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync("displaytemplates", body, ct);
    }

    [McpServerTool(Name = "update_display_template")]
    [Description("Update an existing display template using merge-patch. Only include the fields you want to change.")]
    public static async Task<string> UpdateDisplayTemplate(
        OptimizelyApiClient client,
        [Description("The display template ID")] string id,
        [Description("JSON merge-patch body with fields to update")] string body,
        CancellationToken ct = default)
    {
        return await client.PatchAsync($"displaytemplates/{Uri.EscapeDataString(id)}", body, ct);
    }

    [McpServerTool(Name = "delete_display_template")]
    [Description("Delete a display template by its ID.")]
    public static async Task<string> DeleteDisplayTemplate(
        OptimizelyApiClient client,
        [Description("The display template ID")] string id,
        CancellationToken ct = default)
    {
        return await client.DeleteAsync($"displaytemplates/{Uri.EscapeDataString(id)}", ct);
    }
}

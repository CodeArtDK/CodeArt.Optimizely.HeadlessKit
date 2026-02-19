using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class PropertyFormatTools
{
    [McpServerTool(Name = "list_property_formats")]
    [Description("List all available property formats (data types) in the CMS. These define the allowed data types for content type properties (e.g. 'shortString', 'longString', 'boolean', 'contentReference', 'contentArea', etc.).")]
    public static async Task<string> ListPropertyFormats(
        OptimizelyApiClient client,
        CancellationToken ct = default)
    {
        return await client.GetAsync("propertyformats", ct);
    }

    [McpServerTool(Name = "get_property_format")]
    [Description("Get a single property format by its ID. Returns details about the data type.")]
    public static async Task<string> GetPropertyFormat(
        OptimizelyApiClient client,
        [Description("The property format ID")] string id,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"propertyformats/{Uri.EscapeDataString(id)}", ct);
    }
}

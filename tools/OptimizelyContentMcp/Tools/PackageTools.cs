using System.ComponentModel;
using ModelContextProtocol.Server;
using OptimizelyContentMcp.Client;

namespace OptimizelyContentMcp.Tools;

[McpServerToolType]
public static class PackageTools
{
    [McpServerTool(Name = "get_package_status")]
    [Description("Get the status of a package import/export operation by its key. Returns progress and completion status.")]
    public static async Task<string> GetPackageStatus(
        OptimizelyApiClient client,
        [Description("The package operation key")] string key,
        CancellationToken ct = default)
    {
        return await client.GetAsync($"experimental/packages/{Uri.EscapeDataString(key)}", ct);
    }

    [McpServerTool(Name = "export_package")]
    [Description("Export content as a package. Returns a package key that can be used to check export status and download the result.")]
    public static async Task<string> ExportPackage(
        OptimizelyApiClient client,
        CancellationToken ct = default)
    {
        return await client.GetAsync("experimental/packages", ct);
    }

    [McpServerTool(Name = "import_package")]
    [Description("Import a content package. The body should contain the package data to import.")]
    public static async Task<string> ImportPackage(
        OptimizelyApiClient client,
        [Description("JSON body with the package import data")] string body,
        CancellationToken ct = default)
    {
        return await client.PostAsync("experimental/packages", body, ct);
    }
}

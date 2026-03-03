using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using OptimizelyContentMcp.Client;

var builder = Host.CreateApplicationBuilder(args);

// MCP stdio transport requires logging to stderr only
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

// Register OAuth2 token manager and API client
builder.Services.AddSingleton<OAuth2TokenManager>();
builder.Services.AddHttpClient<OptimizelyApiClient>();
builder.Services.AddHttpClient("OAuth2");

// Configure MCP server with stdio transport and auto-discover tools
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "optimizely-cms",
            Version = "1.0.0"
        };
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

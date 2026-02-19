using System.Text;
using System.Text.Json;

namespace OptimizelyContentMcp.Client;

/// <summary>
/// Generic HTTP client for the Optimizely SaaS CMS REST API.
/// Returns pretty-printed JSON on success or "ERROR {statusCode}: {body}" on failure.
/// </summary>
public sealed class OptimizelyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly OAuth2TokenManager _tokenManager;
    private readonly string _baseUrl;
    private readonly string _apiVersion;

    private static readonly JsonSerializerOptions PrettyJson = new() { WriteIndented = true };

    public OptimizelyApiClient(HttpClient httpClient, OAuth2TokenManager tokenManager)
    {
        _httpClient = httpClient;
        _tokenManager = tokenManager;
        _baseUrl = (Environment.GetEnvironmentVariable("OPTIMIZELY_API_BASE_URL") ?? "https://api.cms.optimizely.com/")
            .TrimEnd('/');
        _apiVersion = Environment.GetEnvironmentVariable("OPTIMIZELY_API_VERSION") ?? "preview3";
    }

    public Task<string> GetAsync(string path, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Get, path, body: null, ct);

    public Task<string> PostAsync(string path, string? body, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, path, body, ct);

    public Task<string> PatchAsync(string path, string body, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Patch, path, body, ct, contentType: "application/merge-patch+json");

    public Task<string> DeleteAsync(string path, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Delete, path, body: null, ct);

    /// <summary>
    /// POST to an action endpoint (e.g., content/{key}:copy).
    /// </summary>
    public Task<string> PostActionAsync(string path, string? body, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, path, body, ct);

    private async Task<string> SendAsync(HttpMethod method, string path, string? body, CancellationToken ct, string contentType = "application/json")
    {
        var url = BuildUrl(path);
        var request = new HttpRequestMessage(method, url);

        var token = await _tokenManager.GetAccessTokenAsync(ct);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        if (body is not null)
        {
            request.Content = new StringContent(body, Encoding.UTF8, contentType);
        }

        try
        {
            var response = await _httpClient.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                return $"ERROR {(int)response.StatusCode}: {responseBody}";

            // Pretty-print JSON if possible
            return PrettyPrintJson(responseBody);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }

    private string BuildUrl(string path)
    {
        // Experimental paths are passed through as-is; stable paths get version prefix
        if (path.StartsWith("experimental/", StringComparison.OrdinalIgnoreCase))
            return $"{_baseUrl}/api/{path}";

        return $"{_baseUrl}/api/{_apiVersion}/{path}";
    }

    private static string PrettyPrintJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, PrettyJson);
        }
        catch
        {
            return json;
        }
    }
}

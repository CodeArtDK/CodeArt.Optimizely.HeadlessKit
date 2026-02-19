using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace OptimizelyContentMcp.Client;

/// <summary>
/// Manages OAuth2 client-credentials tokens with thread-safe caching.
/// </summary>
public sealed class OAuth2TokenManager
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _tokenEndpoint;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _accessToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    private static readonly TimeSpan ExpiryBuffer = TimeSpan.FromSeconds(30);

    public OAuth2TokenManager(IHttpClientFactory httpClientFactory)
    {
        _clientId = Environment.GetEnvironmentVariable("OPTIMIZELY_CLIENT_ID")
            ?? throw new InvalidOperationException("OPTIMIZELY_CLIENT_ID environment variable is required.");
        _clientSecret = Environment.GetEnvironmentVariable("OPTIMIZELY_CLIENT_SECRET")
            ?? throw new InvalidOperationException("OPTIMIZELY_CLIENT_SECRET environment variable is required.");
        _tokenEndpoint = "https://login.optimizely.com/oauth2/token";
        _httpClient = httpClientFactory.CreateClient("OAuth2");
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
            return _accessToken;

        await _semaphore.WaitAsync(ct);
        try
        {
            // Double-check after acquiring lock
            if (_accessToken is not null && DateTimeOffset.UtcNow < _expiresAt)
                return _accessToken;

            var request = new HttpRequestMessage(HttpMethod.Post, _tokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _clientId,
                    ["client_secret"] = _clientSecret
                })
            };

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(ct)
                ?? throw new InvalidOperationException("Failed to deserialize token response.");

            _accessToken = tokenResponse.AccessToken;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn) - ExpiryBuffer;

            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "";
    }
}

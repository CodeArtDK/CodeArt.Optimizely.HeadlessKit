using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Clients
{
    /// <summary>
    /// HTTP client for the Optimizely SaaS CMS REST API. Uses OAuth2 bearer token authentication.
    /// </summary>
    public class SaaSCMSClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Merge-patch requires explicit nulls to clear fields on the server,
        /// so we must NOT skip null values during serialization.
        /// </summary>
        private static readonly JsonSerializerOptions MergePatchJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly SaaSCMSSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaaSCMSClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client configured with OAuth2 token management.</param>
        /// <param name="settings">The SaaS CMS settings.</param>
        public SaaSCMSClient(HttpClient httpClient, IOptions<SaaSCMSSettings> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Lists all content types from the CMS API, handling pagination automatically.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The complete list of content types.</returns>
        public async Task<SaaSContentTypeListResponse> ListContentTypes(CancellationToken cancellationToken = default)
        {
            var endpoint = BuildEndpoint("contenttypes");
            if (!string.IsNullOrWhiteSpace(_settings.ContentTypeSources))
            {
                endpoint = AppendQueryParam(endpoint, "sources", _settings.ContentTypeSources);
            }

            var result = await _httpClient.GetFromJsonAsync<SaaSContentTypeListResponse>(endpoint, JsonOptions, cancellationToken)
                ?? new SaaSContentTypeListResponse();

            while (result.Items.Count < result.TotalItemCount)
            {
                var nextPage = result.PageIndex + 1;
                var nextEndpoint = AppendQueryParam(endpoint, "pageIndex", nextPage.ToString());
                var page = await _httpClient.GetFromJsonAsync<SaaSContentTypeListResponse>(nextEndpoint, JsonOptions, cancellationToken);
                if (page?.Items == null || page.Items.Count == 0)
                    break;
                result.Items.AddRange(page.Items);
                result.PageIndex = page.PageIndex;
            }

            return result;
        }

        /// <summary>
        /// Retrieves a single content type by key. Returns null if not found.
        /// </summary>
        /// <param name="key">The content type key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The content type, or null if not found.</returns>
        public async Task<SaaSContentType?> GetContentType(string key, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BuildEndpoint("contenttypes")}/{Uri.EscapeDataString(key)}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SaaSContentType>(JsonOptions, cancellationToken);
        }

        /// <summary>
        /// Retrieves a single display template by key. Returns null if not found.
        /// </summary>
        /// <param name="key">The display template key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The display template, or null if not found.</returns>
        public async Task<SaaSDisplayTemplate?> GetDisplayTemplate(string key, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BuildEndpoint("displaytemplates")}/{Uri.EscapeDataString(key)}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SaaSDisplayTemplate>(JsonOptions, cancellationToken);
        }

        /// <summary>
        /// Creates a new content type in the CMS API.
        /// </summary>
        /// <param name="contentType">The content type definition to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task CreateContentType(SaaSContentType contentType, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(BuildEndpoint("contenttypes"), contentType, JsonOptions, cancellationToken);
            await EnsureSuccess(response, "content type", contentType.Key ?? "(unknown)", cancellationToken);
        }

        /// <summary>
        /// Updates an existing content type using HTTP PATCH with merge-patch+json.
        /// </summary>
        /// <param name="key">The content type key to update.</param>
        /// <param name="contentType">The content type definition containing the changed fields.</param>
        /// <param name="ignoreDataLossWarnings">When true, suppresses API data loss warnings.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task UpdateContentType(string key, SaaSContentType contentType, bool ignoreDataLossWarnings = false, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(contentType, JsonOptions);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/merge-patch+json");
            var url = $"{BuildEndpoint("contenttypes")}/{key}";
            var request = new HttpRequestMessage(HttpMethod.Patch, url) { Content = content };
            if (ignoreDataLossWarnings)
                request.Headers.TryAddWithoutValidation("cms-ignore-data-loss-warnings", "true");
            var response = await _httpClient.SendAsync(request, cancellationToken);
            await EnsureSuccess(response, "content type", key, cancellationToken);
        }

        /// <summary>
        /// Lists all display templates from the CMS API, handling pagination automatically.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The complete list of display templates.</returns>
        public async Task<SaaSDisplayTemplateListResponse> ListDisplayTemplates(CancellationToken cancellationToken = default)
        {
            var endpoint = BuildEndpoint("displaytemplates");
            var result = await _httpClient.GetFromJsonAsync<SaaSDisplayTemplateListResponse>(endpoint, JsonOptions, cancellationToken)
                ?? new SaaSDisplayTemplateListResponse();

            while (result.Items.Count < result.TotalItemCount)
            {
                var nextPage = result.PageIndex + 1;
                var nextEndpoint = AppendQueryParam(endpoint, "pageIndex", nextPage.ToString());
                var page = await _httpClient.GetFromJsonAsync<SaaSDisplayTemplateListResponse>(nextEndpoint, JsonOptions, cancellationToken);
                if (page?.Items == null || page.Items.Count == 0)
                    break;
                result.Items.AddRange(page.Items);
                result.PageIndex = page.PageIndex;
            }

            return result;
        }

        /// <summary>
        /// Creates a new display template in the CMS API.
        /// </summary>
        /// <param name="displayTemplate">The display template definition to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task CreateDisplayTemplate(SaaSDisplayTemplate displayTemplate, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(BuildEndpoint("displaytemplates"), displayTemplate, JsonOptions, cancellationToken);
            await EnsureSuccess(response, "display template", displayTemplate.Key ?? "(unknown)", cancellationToken);
        }

        /// <summary>
        /// Updates an existing display template using HTTP PATCH with merge-patch+json.
        /// </summary>
        /// <param name="key">The display template key to update.</param>
        /// <param name="displayTemplate">The display template definition with updated values.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task UpdateDisplayTemplate(string key, SaaSDisplayTemplate displayTemplate, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(displayTemplate, MergePatchJsonOptions);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/merge-patch+json");
            var response = await _httpClient.PatchAsync($"{BuildEndpoint("displaytemplates")}/{key}", content, cancellationToken);
            await EnsureSuccess(response, "display template", key, cancellationToken);
        }

        private static string AppendQueryParam(string url, string key, string value)
        {
            var separator = url.Contains('?') ? '&' : '?';
            return $"{url}{separator}{key}={Uri.EscapeDataString(value)}";
        }

        private string BuildEndpoint(string endpoint)
        {
            var prefix = _settings.ApiPathPrefix?.Trim('/');
            var relative = endpoint.TrimStart('/');
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return relative;
            }

            return $"{prefix}/{relative}";
        }

        private static async Task EnsureSuccess(HttpResponseMessage response, string resourceType, string key, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to sync {resourceType} '{key}'. Status: {(int)response.StatusCode} {response.StatusCode}. {content}");
        }
    }
}

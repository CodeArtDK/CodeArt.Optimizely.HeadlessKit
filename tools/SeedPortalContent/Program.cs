using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// ── Configuration ──────────────────────────────────────────────────
// These match the credentials in the sample sites' user secrets.
// Replace with your own if using a different CMS instance.
const string ClientId = "ffe9e67b59464a3d910cb44051a44a2e";
const string ClientSecret = "hVkrKDUZesTyBEHZJjrDMUAkASB0vQYKNRjWQCm5NJIHPSNM";
const string ApiBase = "https://api.cms.optimizely.com";
const string TypePrefix = "preview3";

var http = new HttpClient();
var jsonOpts = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

// ── Token Management ───────────────────────────────────────────────
string? _token = null;
DateTime _tokenExpiry = DateTime.MinValue;

async Task<string> GetToken()
{
    if (_token != null && DateTime.UtcNow < _tokenExpiry)
        return _token;
    var resp = await http.PostAsync($"{ApiBase}/oauth/token",
        new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = ClientId,
            ["client_secret"] = ClientSecret
        }));
    resp.EnsureSuccessStatusCode();
    var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
    _token = json.GetProperty("access_token").GetString()!;
    _tokenExpiry = DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32() - 30);
    Console.WriteLine("  [Token refreshed]");
    return _token;
}

async Task<(bool ok, int status, string body)> Api(HttpMethod method, string path, object? body = null, string contentType = "application/json")
{
    var token = await GetToken();
    var req = new HttpRequestMessage(method, $"{ApiBase}/{TypePrefix}/{path}");
    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    if (body != null)
    {
        var json = JsonSerializer.Serialize(body, jsonOpts);
        req.Content = new StringContent(json, Encoding.UTF8, contentType);
    }
    var resp = await http.SendAsync(req);
    var respBody = await resp.Content.ReadAsStringAsync();
    return (resp.IsSuccessStatusCode, (int)resp.StatusCode, respBody);
}

// ══════════════════════════════════════════════════════════════════
// STEP 1: Create Element Content Types
// ══════════════════════════════════════════════════════════════════
Console.WriteLine("=== Step 1: Create Element Content Types ===");

var elementDefs = new (string key, string displayName, Dictionary<string, object> properties)[]
{
    ("WelcomeBannerElement", "Welcome Banner Element", new()
    {
        ["greetingTemplate"] = new { type = "string", format = "shortString", displayName = "Greeting Template", localized = true, sortOrder = 10 },
        ["subtitle"] = new { type = "string", format = "shortString", displayName = "Subtitle", localized = true, sortOrder = 20 },
        ["iconClass"] = new { type = "string", format = "shortString", displayName = "Icon Class", sortOrder = 30 }
    }),
    ("UsageStatElement", "Usage Stat Element", new()
    {
        ["label"] = new { type = "string", format = "shortString", displayName = "Label", localized = true, sortOrder = 10 },
        ["unit"] = new { type = "string", format = "shortString", displayName = "Unit", sortOrder = 20 },
        ["iconClass"] = new { type = "string", format = "shortString", displayName = "Icon Class", sortOrder = 30 },
        ["maxValue"] = new { type = "integer", displayName = "Max Value", sortOrder = 40 },
        ["dataKey"] = new { type = "string", format = "shortString", displayName = "Data Key", sortOrder = 50 }
    }),
    ("InfoCardElement", "Info Card Element", new()
    {
        ["title"] = new { type = "string", format = "shortString", displayName = "Title", localized = true, sortOrder = 10 },
        ["description"] = new { type = "string", displayName = "Description", localized = true, sortOrder = 20 },
        ["iconClass"] = new { type = "string", format = "shortString", displayName = "Icon Class", sortOrder = 30 },
        ["link"] = new { type = "url", displayName = "Link", sortOrder = 40 },
        ["linkText"] = new { type = "string", format = "shortString", displayName = "Link Text", sortOrder = 50 }
    }),
    ("AnnouncementElement", "Announcement Element", new()
    {
        ["title"] = new { type = "string", format = "shortString", displayName = "Title", localized = true, sortOrder = 10 },
        ["body"] = new { type = "string", displayName = "Body", localized = true, sortOrder = 20 },
        ["severity"] = new { type = "string", format = "shortString", displayName = "Severity", sortOrder = 30 },
        ["iconClass"] = new { type = "string", format = "shortString", displayName = "Icon Class", sortOrder = 40 }
    }),
    ("AccountDetailElement", "Account Detail Element", new()
    {
        ["label"] = new { type = "string", format = "shortString", displayName = "Label", localized = true, sortOrder = 10 },
        ["iconClass"] = new { type = "string", format = "shortString", displayName = "Icon Class", sortOrder = 20 },
        ["displayFormat"] = new { type = "string", format = "shortString", displayName = "Display Format", sortOrder = 30 },
        ["dataKey"] = new { type = "string", format = "shortString", displayName = "Data Key", sortOrder = 40 }
    }),
    ("BillingHistoryElement", "Billing History Element", new()
    {
        ["title"] = new { type = "string", format = "shortString", displayName = "Title", localized = true, sortOrder = 10 },
        ["dateHeader"] = new { type = "string", format = "shortString", displayName = "Date Header", localized = true, sortOrder = 20 },
        ["descriptionHeader"] = new { type = "string", format = "shortString", displayName = "Description Header", localized = true, sortOrder = 30 },
        ["amountHeader"] = new { type = "string", format = "shortString", displayName = "Amount Header", localized = true, sortOrder = 40 },
        ["statusHeader"] = new { type = "string", format = "shortString", displayName = "Status Header", localized = true, sortOrder = 50 },
        ["emptyMessage"] = new { type = "string", displayName = "Empty Message", localized = true, sortOrder = 60 }
    })
};

var containable = elementDefs.Select(e => e.key).ToArray();

foreach (var (key, displayName, properties) in elementDefs)
{
    var (exists, _, _) = await Api(HttpMethod.Get, $"contenttypes/{key}");
    if (exists) { Console.WriteLine($"  [EXISTS] {key}"); continue; }
    var (ok, _, resp) = await Api(HttpMethod.Post, "contenttypes", new { key, displayName, baseType = "_component", sortOrder = 100,
        features = new[] { "localization", "versioning", "publishPeriod" }, usages = new[] { "property", "instance" }, properties });
    Console.WriteLine($"  [{(ok ? "CREATED" : "FAILED")}] {key} {(ok ? "" : resp)}");
}

// ══════════════════════════════════════════════════════════════════
// STEP 2: Create Experience Content Types
// ══════════════════════════════════════════════════════════════════
Console.WriteLine("\n=== Step 2: Create Experience Content Types ===");

foreach (var (key, displayName, properties) in new[]
{
    ("PortalDashboard", "Portal Dashboard", new Dictionary<string, object>
    {
        ["portalName"] = new { type = "string", format = "shortString", displayName = "Portal Name", localized = true, sortOrder = 10, maxLength = 100 },
        ["supportEmail"] = new { type = "string", format = "shortString", displayName = "Support Email", sortOrder = 20 }
    }),
    ("PortalPage", "Portal Page", new Dictionary<string, object>
    {
        ["pageTitle"] = new { type = "string", format = "shortString", displayName = "Page Title", localized = true, sortOrder = 10, maxLength = 200 },
        ["pageDescription"] = new { type = "string", format = "shortString", displayName = "Page Description", localized = true, sortOrder = 20, maxLength = 500 }
    })
})
{
    var (exists, _, _) = await Api(HttpMethod.Get, $"contenttypes/{key}");
    if (exists) { Console.WriteLine($"  [EXISTS] {key}"); continue; }
    var (ok, _, resp) = await Api(HttpMethod.Post, "contenttypes", new { key, displayName, baseType = "_experience", sortOrder = 200,
        features = new[] { "localization", "versioning", "publishPeriod" }, usages = new[] { "property", "instance" },
        mayContainTypes = containable, properties });
    Console.WriteLine($"  [{(ok ? "CREATED" : "FAILED")}] {key} {(ok ? "" : resp)}");
}

// ══════════════════════════════════════════════════════════════════
// STEP 3: Verify
// ══════════════════════════════════════════════════════════════════
Console.WriteLine("\n=== Step 3: Verify Content Types ===");

foreach (var key in containable.Concat(new[] { "PortalDashboard", "PortalPage" }))
{
    var (ok, _, _) = await Api(HttpMethod.Get, $"contenttypes/{key}");
    Console.WriteLine($"  {key}: {(ok ? "OK" : "MISSING")}");
}

Console.WriteLine("\n=== Content Type Setup Complete ===");
Console.WriteLine();
Console.WriteLine("All 8 portal content types have been created in the CMS.");
Console.WriteLine("Next steps to set up portal content:");
Console.WriteLine();
Console.WriteLine("  1. Run the Blazor sample to sync types: dotnet run --project samples/HeadlessKit.Sample.Blazor");
Console.WriteLine("  2. Open the CMS Visual Builder and create:");
Console.WriteLine("     - A PortalDashboard page with routeSegment 'portal'");
Console.WriteLine("     - A PortalPage 'Account' with routeSegment 'account' (under dashboard)");
Console.WriteLine("     - A PortalPage 'Billing' with routeSegment 'billing' (under dashboard)");
Console.WriteLine("  3. Add elements to each page using the Visual Builder's composition editor");
Console.WriteLine("  4. Publish all pages");
Console.WriteLine();
Console.WriteLine("The Blazor app shows fallback content until CMS pages are configured.");

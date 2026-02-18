# Getting Started

This guide walks you through setting up a headless Optimizely CMS (SaaS) site using CodeArt.Optimizely.HeadlessKit.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- An Optimizely CMS SaaS account with:
  - **Client ID** and **Client Secret** (for the REST API / type builder)
  - **Graph Single Key** (for Optimizely Graph content queries)

## 1. Install

```bash
dotnet add package CodeArt.Optimizely.HeadlessKit
```

## 2. Configure

Add both `SaaSCMS` and `OptimizelyGraph` sections to `appsettings.json`:

```json
{
  "SaaSCMS": {
    "ApiBaseUrl": "https://api.cms.optimizely.com/",
    "ApiPathPrefix": "preview3",
    "ClientId": "<your-client-id>",
    "ClientSecret": "<your-client-secret>",
    "SyncOnStartup": true,
    "UpdateExistingContentTypes": true,
    "UpdateExistingDisplayTemplates": true
  },
  "OptimizelyGraph": {
    "GraphEndpoint": "https://cg.optimizely.com/content/v2",
    "SingleKey": "<your-single-key>",
    "CmsAppUrl": "https://app-xxxxx.cms.optimizely.com",
    "CacheDurationSeconds": 300
  }
}
```

> **Tip:** Use [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for `ClientId`, `ClientSecret`, and `SingleKey` in development.

### Configuration Reference

| Section | Key | Default | Description |
|---------|-----|---------|-------------|
| SaaSCMS | `ClientId` | *(required)* | OAuth2 client ID for the CMS REST API |
| SaaSCMS | `ClientSecret` | *(required)* | OAuth2 client secret |
| SaaSCMS | `ApiBaseUrl` | `https://api.cms.optimizely.com/` | CMS API base URL |
| SaaSCMS | `ApiPathPrefix` | `preview3` | API version prefix |
| SaaSCMS | `SyncOnStartup` | `true` | Sync content types on application startup |
| SaaSCMS | `UpdateExistingContentTypes` | `true` | Update remote types if local definitions changed |
| SaaSCMS | `UpdateExistingDisplayTemplates` | `true` | Update remote display templates if changed |
| SaaSCMS | `IgnoreDataLossWarnings` | `false` | Suppress API data loss warnings on updates |
| OptimizelyGraph | `SingleKey` | *(required)* | Optimizely Graph authentication key |
| OptimizelyGraph | `GraphEndpoint` | `https://cg.optimizely.com/content/v2` | Graph API endpoint |
| OptimizelyGraph | `CmsAppUrl` | *(optional)* | CMS app URL for preview support |
| OptimizelyGraph | `CacheDurationSeconds` | `300` | Content cache TTL (0 to disable) |
| OptimizelyGraph | `DebugLogging` | `false` | Log GraphQL queries and responses |

## 3. Register Services

In `Program.cs`:

```csharp
using CodeArt.Optimizely.HeadlessKit.Mvc;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Register TypeBuilder (content type sync) and Graph services (content client + MVC)
builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
builder.Services.AddOptimizelyGraph(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Add CMS preview middleware (must be before UseRouting)
app.UseCmsPreview();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map catch-all dynamic route for CMS content
app.MapDynamicPageRoute<ContentRouteTransformer>("{**path}");
app.MapRazorPages();

// Initialize services (TemplateCoordinator, etc.)
await app.InitializeServicesAsync();

app.Run();
```

## 4. Define a Content Type

Content types are .NET POCOs annotated with `[ContentType]`. They are automatically discovered and synced to the CMS on startup.

```csharp
using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.ComponentModel.DataAnnotations;

// Experience pages use Visual Builder composition
[ContentType("StandardPage", BaseTypes.Experience, MayContainTypes = new[] {
    "HeroElement", "BannerElement", "EditorialElement"
})]
public class StandardPage : GraphExperience
{
    [CultureSpecific]
    [MaxLength(200)]
    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string MetaTitle { get; set; }

    [MaxLength(500)]
    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string MetaDescription { get; set; }
}
```

For elements (components used inside compositions):

```csharp
[ContentType("HeroElement", BaseTypes.Element)]
public class HeroElement : GraphBlock
{
    [CultureSpecific]
    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string Title { get; set; }

    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string Subtitle { get; set; }

    public GraphContentReference BackgroundImage { get; set; }

    public GraphContentUrl ButtonLink { get; set; }
}
```

See [Content Types](content-types.md) for the full attribute reference.

## 5. Create a Page Template

Register a Razor Page as a rendering template for a content type using `[TemplateDescriptor]`:

**Pages/StandardPage.cshtml.cs:**
```csharp
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using CodeArt.Optimizely.HeadlessKit.Mvc.Models;

[TemplateDescriptor(typeof(StandardPage))]
public class StandardPageModel : ContentPage<StandardPage> { }
```

**Pages/StandardPage.cshtml:**
```html
@page
@model StandardPageModel
@{
    ViewData["Title"] = Model.CurrentContent?.MetaTitle;
}

<graph-content-area composition="@Model.CurrentContent?.Composition" />
```

## 6. Register Tag Helpers

Add to `Pages/_ViewImports.cshtml`:

```html
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, CodeArt.Optimizely.HeadlessKit
```

## 7. Import Sample Content (Optional)

To see the sample site in action with real content, import the included content package:

1. Run the site once to sync content types to your CMS (the TypeBuilder does this on startup)
2. In your Optimizely CMS, go to **Settings** > **Import Data**
3. Upload `meridiandigital.episerverdata` from `samples/HeadlessKit.Sample.RazorPages/ContentPackage/`
4. Wait a few minutes for the content to appear in Optimizely Graph

See the [ContentPackage README](../samples/HeadlessKit.Sample.RazorPages/ContentPackage/README.md) for details.

## 8. Run

```bash
dotnet run
```

On startup, the TypeBuilder will:
1. Scan assemblies for `[ContentType]` and `[DisplayTemplate]` annotations
2. Compare with existing types in the CMS API
3. Create missing types and update changed ones
4. Log sync results

Content is then served through the catch-all route, loaded from Optimizely Graph, and rendered via the matching page template.

## Next Steps

- [Content Types](content-types.md) -- Full attribute reference and property type mapping
- [Display Templates](display-templates.md) -- Configure editor display settings
- [Content Querying](content-querying.md) -- Fluent queries, filters, search, and pagination
- [MVC Integration](mvc-integration.md) -- Routing, tag helpers, view components, and preview

# CodeArt.Optimizely.HeadlessKit

A .NET 10 library for building headless sites with Optimizely CMS (SaaS). Combines content type definition, GraphQL content querying, and ASP.NET Core MVC rendering into a single NuGet package.

## Features

- **Type Builder** -- Define CMS content types as .NET POCOs with attributes. Types are automatically synced to Optimizely CMS on startup via REST API (create/update only, never deletes).
- **Content Client** -- Query content from Optimizely Graph (GraphQL). Auto-generates queries from registered types, supports fluent query building with filters, search with facets and pagination.
- **MVC Integration** -- Content routing, tag helpers (`<graph-image>`, `<graph-link>`, `<graph-richtext>`, `<graph-content-area>`), view components for composition rendering, and CMS preview support.

## Quick Start

### 1. Install

```bash
dotnet add package CodeArt.Optimizely.HeadlessKit
```

### 2. Configure

Add to `appsettings.json`:

```json
{
  "SaaSCMS": {
    "ApiBaseUrl": "https://api.cms.optimizely.com/",
    "ApiPathPrefix": "preview3",
    "ClientId": "<your-client-id>",
    "ClientSecret": "<your-client-secret>",
    "SyncOnStartup": true
  },
  "OptimizelyGraph": {
    "GraphEndpoint": "https://cg.optimizely.com/content/v2",
    "SingleKey": "<your-single-key>"
  }
}
```

### 3. Register Services

```csharp
builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
builder.Services.AddOptimizelyGraph(builder.Configuration);
```

### 4. Define Content Types

```csharp
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

[ContentType("heroElement", BaseTypes.Element)]
public class HeroElement : ComponentBase
{
    [CMSProperty(DisplayName = "Title", Format = PropertyFormats.ShortString)]
    public string? Title { get; set; }

    [CMSProperty(DisplayName = "Image", Format = PropertyFormats.ContentReference)]
    public GraphContentReference? Image { get; set; }
}
```

### 5. Query Content

Content is queried automatically via content routing, or manually:

```csharp
// Via the content repository
var content = await contentRepository.GetByPath<MyPage>("/en/my-page");

// Via the fluent query builder
var results = await graphQueryBuilder
    .ForType<ArticlePage>()
    .Where(x => x.Title, "contains", "news")
    .OrderBy("_metadata/published", descending: true)
    .Take(10)
    .ExecuteAsync();
```

### 6. Render with Tag Helpers

```html
@addTagHelper *, CodeArt.Optimizely.HeadlessKit

<graph-image content="@Model.Image" width="800" class="hero-img" />
<graph-link content="@Model.Link">Click here</graph-link>
<graph-richtext content="@Model.Body" />
<graph-content-area composition="@Model.Composition" />
```

## Sample Site

See `samples/HeadlessKit.Sample.RazorPages/` for a full working example with 26 element types, 4 page types, display templates, and Razor Pages rendering.

To run:

```bash
dotnet run --project samples/HeadlessKit.Sample.RazorPages/HeadlessKit.Sample.RazorPages.csproj
```

> **Note:** You need valid Optimizely CMS SaaS credentials configured via user secrets or `appsettings.json`.

## Building

```bash
# Build the solution
dotnet build src/CodeArt.Optimizely.HeadlessKit.sln

# Run tests
dotnet test test/CodeArt.Optimizely.HeadlessKit.Tests/CodeArt.Optimizely.HeadlessKit.Tests.csproj

# Create NuGet package
dotnet pack src/CodeArt.Optimizely.HeadlessKit/CodeArt.Optimizely.HeadlessKit.csproj
```

## License

MIT -- see [LICENSE](LICENSE) for details.

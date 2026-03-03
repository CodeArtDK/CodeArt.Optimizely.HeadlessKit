# CodeArt.Optimizely.HeadlessKit

A .NET 10 library for building headless sites with Optimizely CMS (SaaS). Combines content type definition, GraphQL content querying, and ASP.NET Core MVC rendering into a single NuGet package.

## Features

- **Type Builder** -- Define CMS content types as .NET POCOs with attributes. Types are automatically synced to Optimizely CMS on startup via REST API (create/update only, never deletes).
- **Content Client** -- Query content from Optimizely Graph (GraphQL). Auto-generates queries from registered types, supports fluent query building with filters, search with facets and pagination.
- **MVC Integration** -- Content routing, tag helpers (`<graph-image>`, `<graph-link>`, `<graph-rich-text>`, `<graph-content-area>`), view components for composition rendering, and CMS preview support.

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
using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

[ContentType("HeroElement", BaseTypes.Element)]
public class HeroElement : GraphBlock
{
    [CultureSpecific]
    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string Title { get; set; }

    public GraphContentReference BackgroundImage { get; set; }
    public GraphContentUrl ButtonLink { get; set; }
}
```

### 5. Query Content

Content is queried automatically via content routing, or manually:

```csharp
// Via the content repository (cached)
var page = await contentRepository.GetContentByPath<StandardPage>("/en/about");

// Via the fluent query builder
var articles = await GraphQuery.For<ArticlePage>(client)
    .Where(f => f.Metadata.Status.Eq("Published"))
    .OrderBy(a => a.MetaData.Published, OrderDirection.DESC)
    .Take(10)
    .ToListAsync();
```

### 6. Render with Tag Helpers

```html
@addTagHelper *, CodeArt.Optimizely.HeadlessKit

<graph-image content="@Model.Image" width="800" alt="Hero" css-class="hero-img" />
<graph-link content="@Model.Link" css-class="btn">Click here</graph-link>
<graph-rich-text content="@Model.Body" />
<graph-content-area composition="@Model.CurrentContent?.Composition" />
```

## Sample Sites

Two complete sample sites are included, both with 26 element types, 4 page types, and display templates:

| Sample | Approach | Path |
|--------|----------|------|
| **Razor Pages** | `ContentPage<T>` base class, `MapDynamicPageRoute` | `samples/HeadlessKit.Sample.RazorPages/` |
| **MVC** | `ContentControllerBase<T>` base class, `MapContentControllerRoute` | `samples/HeadlessKit.Sample.Mvc/` |

Both sites share the same content types, element models, display templates, look & feel, and CSS. They differ only in the rendering approach (Razor Pages vs MVC Controllers).

A **content package** is included at `samples/HeadlessKit.Sample.RazorPages/ContentPackage/` -- import it into your CMS to get sample pages and elements that work with both sample sites out of the box.

To run:

```bash
# Razor Pages sample
dotnet run --project samples/HeadlessKit.Sample.RazorPages/HeadlessKit.Sample.RazorPages.csproj

# MVC sample
dotnet run --project samples/HeadlessKit.Sample.Mvc/HeadlessKit.Sample.Mvc.csproj
```

> **Note:** You need valid Optimizely CMS SaaS credentials configured via user secrets or `appsettings.json`.

## MCP Server for AI Assistants

The **Optimizely CMS MCP Server** lets AI assistants (Claude, Copilot, etc.) manage your CMS content directly — create pages, define content types, build Visual Builder experiences, handle versions, and more through natural language.

Download the latest self-contained executable from [GitHub Releases](https://github.com/CodeArtDK/CodeArt.Optimizely.HeadlessKit/releases) (no .NET runtime required), then configure your AI client:

```json
{
  "mcpServers": {
    "optimizely-cms": {
      "command": "/path/to/OptimizelyContentMcp",
      "env": {
        "OPTIMIZELY_CLIENT_ID": "<your-client-id>",
        "OPTIMIZELY_CLIENT_SECRET": "<your-client-secret>"
      }
    }
  }
}
```

> See the full [MCP Server README](tools/OptimizelyContentMcp/README.md) for setup instructions for Claude Desktop, Claude Code, and VS Code. The included [SKILL.md](tools/OptimizelyContentMcp/SKILL.md) teaches AI assistants how to use the tools effectively.

## Documentation

- [Getting Started](docs/getting-started.md) -- Full setup guide
- [Content Types](docs/content-types.md) -- Attribute reference and type sync
- [Display Templates](docs/display-templates.md) -- Editor display settings
- [Content Querying](docs/content-querying.md) -- Fluent queries, filters, search
- [MVC Integration](docs/mvc-integration.md) -- Routing, tag helpers, view components, preview

### AI Assistant Instructions

- [Using HeadlessKit](docs/ai-instructions/using-headlesskit.md) -- For AI coding assistants helping developers
- [Optimizely SaaS API](docs/ai-instructions/optimizely-saas-content-api.md) -- For AI assistants calling CMS APIs directly
- [MCP Server Skill Guide](tools/OptimizelyContentMcp/SKILL.md) -- For AI assistants using the MCP server tools

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

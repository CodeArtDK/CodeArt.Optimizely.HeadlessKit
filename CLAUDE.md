# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CodeArt.Optimizely.HeadlessKit is a .NET 10 library for building headless sites with Optimizely CMS (SaaS). It combines content type definition, GraphQL content querying, and ASP.NET Core MVC rendering into a single NuGet package. Sync operations create and update types but never delete, to avoid accidental content loss.

## Build Commands

```bash
# Build entire solution
dotnet build src/CodeArt.Optimizely.HeadlessKit.sln

# Build only the library
dotnet build src/CodeArt.Optimizely.HeadlessKit/CodeArt.Optimizely.HeadlessKit.csproj

# Run tests
dotnet test test/CodeArt.Optimizely.HeadlessKit.Tests/CodeArt.Optimizely.HeadlessKit.Tests.csproj

# Run the sample site (Razor Pages)
dotnet run --project samples/HeadlessKit.Sample.RazorPages/HeadlessKit.Sample.RazorPages.csproj

# Run the sample site (MVC)
dotnet run --project samples/HeadlessKit.Sample.Mvc/HeadlessKit.Sample.Mvc.csproj

# Create NuGet package
dotnet pack src/CodeArt.Optimizely.HeadlessKit/CodeArt.Optimizely.HeadlessKit.csproj
```

## Solution Structure

```
src/CodeArt.Optimizely.HeadlessKit/       <- single merged library (NuGet package)
├── Core/                                  <- shared models, composition, rendering interfaces
│   ├── Models/                            <- GraphContent, GraphPageContent, composition nodes, etc.
│   └── Rendering/                         <- IDisplayTemplateResolver interface
├── TypeBuilder/                           <- CMS type builder (annotations, sync, API client)
│   ├── Annotation/                        <- [ContentType], [CMSProperty], [CultureSpecific], etc.
│   ├── Clients/                           <- SaaSCMSClient (REST API with OAuth2)
│   ├── Models/                            <- SaaSContentType, SaaSCMSSettings, BaseTypes, etc.
│   ├── Serialization/                     <- JSON converters for CMS API types
│   └── Sync/                              <- ContentTypeComparer, SyncReport
├── ContentClient/                         <- GraphQL content query client
│   ├── AutoGraphQueryProvider             <- auto-generates GraphQL queries from registered types
│   ├── ContentGraphClient                 <- main Graph client (query by path, key, version)
│   ├── GraphQueryBuilder                  <- fluent query builder with filters
│   ├── GraphContentRepository             <- IContentRepository implementation
│   └── ContentTypeRegistry                <- maps .NET types to Graph type names
└── Mvc/                                   <- ASP.NET Core MVC integration
    ├── TagHelpers/                         <- <graph-image>, <graph-link>, <graph-richtext>, etc.
    ├── Components/                         <- view components for composition rendering
    ├── Controllers/                        <- ContentControllerBase
    └── Infrastructure/                     <- content routing, model binding, preview middleware

samples/HeadlessKit.Sample.RazorPages/    <- demo Razor Pages site
samples/HeadlessKit.Sample.Mvc/           <- demo MVC site
test/CodeArt.Optimizely.HeadlessKit.Tests/ <- xUnit tests
```

The solution file is at `src/CodeArt.Optimizely.HeadlessKit.sln`.

## Architecture

### Core (`Core/`)
Shared models (`GraphContent`, `GraphContentReference`, `GraphPageContent`, etc.), composition models (`ContentComposition`, `CompositionStructureNode`, `CompositionComponentNode`), and the `IDisplayTemplateResolver` rendering interface.

### TypeBuilder (`TypeBuilder/`)
1. **Registration**: `services.AddSaaSCMSTypeBuilder(configuration)` in `ServiceCollectionExtensions.cs` wires up all services including OAuth2 token management via `IdentityModel`.
2. **Assembly scanning**: `AppDomainScanner` discovers classes annotated with `[ContentType]` and `[DisplayTemplate]` at startup.
3. **API client**: `SaaSCMSClient` is an `HttpClient`-based client with automatic OAuth2 bearer tokens for the SaaS CMS REST API.
4. **Sync orchestration**: `CMSTypeSyncService` compares discovered types against existing API types, then creates/updates as needed. `ContentTypeComparer` handles diff logic and `SyncReport` tracks results.
5. **Hosted service**: `SaaSCMSTypeSyncHostedService` triggers sync at app startup when `SyncOnStartup` is enabled.

### ContentClient (`ContentClient/`)
GraphQL client for querying content from Optimizely Graph. Key classes:
- `ContentGraphClient` — main entry point; queries by path, key, or version
- `AutoGraphQueryProvider` — auto-generates GraphQL queries (with fragments) from registered content types
- `GraphQueryBuilder` — fluent builder for custom queries with `GraphFilter` support
- `SearchQueryBuilder` — search-specific query builder with facets and pagination
- `GraphContentRepository` — `IContentRepository` implementation
- `ContentTypeRegistry` — maps .NET types to Graph type names

### Mvc (`Mvc/`)
ASP.NET Core integration:
- **Content routing**: `ContentRouteTransformer` resolves CMS URLs to content
- **Tag helpers**: `<graph-image>`, `<graph-link>`, `<graph-richtext>`, `<graph-content-area>`, `<cms-preview-script>`
- **View components**: `CompositionRendererViewComponent`, `CompositionNodeViewComponent` for composition rendering
- **Preview**: `CmsPreviewMiddleware` for CMS preview support
- **Template coordination**: `TemplateCoordinator` maps content types to display templates

### Annotation System
- `[ContentType("Key", BaseTypes.Page)]` — Marks a POCO as a content type
- `[CMSProperty(DisplayName = "...", Format = PropertyFormats.ShortString)]` — Property metadata
- `[CultureSpecific]` — Marks properties as localizable
- `[DisplayTemplate]` — Marks display template classes
- `[DisplayTemplateSetting]` / `[DisplayTemplateChoice]` — Display template configuration

### Configuration
Settings live in the `SaaSCMS` section (class: `SaaSCMSSettings`). Required: `ClientId`, `ClientSecret`. Key options: `SyncOnStartup` (default true), `UpdateExistingContentTypes` (default true), `ApiBaseUrl`, `ApiPathPrefix`.

Graph settings live in the `OptimizelyGraph` section (class: `OptimizelyGraphSettings`). Required: `SingleKey`, `GraphEndpoint`.

## Key Conventions

- .NET 10 with nullable reference types and implicit usings enabled
- Root namespace: `CodeArt.Optimizely.HeadlessKit`
- DI registration via `Add*` extension methods on `IServiceCollection`
- Configuration via `IOptions<T>` pattern
- JSON serialization uses `System.Text.Json` with `[JsonPropertyName]` attributes
- Private fields: `_camelCase`; everything else: PascalCase
- Razor SDK (`Microsoft.NET.Sdk.Razor`) used for the library (contains embedded views)
- Key dependencies: `GraphQL.Client` 6.1, `IdentityModel.AspNetCore` 4.3
- Tests use xUnit 2.9 + FluentAssertions 8.8

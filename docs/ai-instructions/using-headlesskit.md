# AI Instructions: Using CodeArt.Optimizely.HeadlessKit

This document provides comprehensive instructions for AI coding assistants (Claude Code, GitHub Copilot, etc.) helping developers build headless Optimizely CMS sites with the HeadlessKit package.

## Package Overview

CodeArt.Optimizely.HeadlessKit is a single .NET 10 NuGet package that provides:
- **TypeBuilder** -- Define CMS content types as annotated .NET POCOs, synced to Optimizely SaaS CMS via REST API
- **ContentClient** -- Query content from Optimizely Graph (GraphQL) with fluent builders and caching
- **Mvc** -- ASP.NET Core integration with content routing, tag helpers, view components, and CMS preview

## Installation & Setup

```bash
dotnet add package CodeArt.Optimizely.HeadlessKit
```

### appsettings.json

```json
{
  "SaaSCMS": {
    "ApiBaseUrl": "https://api.cms.optimizely.com/",
    "ApiPathPrefix": "preview3",
    "ClientId": "<client-id>",
    "ClientSecret": "<client-secret>",
    "SyncOnStartup": true,
    "UpdateExistingContentTypes": true,
    "UpdateExistingDisplayTemplates": true,
    "IgnoreDataLossWarnings": false
  },
  "OptimizelyGraph": {
    "GraphEndpoint": "https://cg.optimizely.com/content/v2",
    "SingleKey": "<single-key>",
    "CmsAppUrl": "https://app-xxxxx.cms.optimizely.com",
    "CacheDurationSeconds": 300,
    "DebugLogging": false
  }
}
```

### Program.cs (Razor Pages)

```csharp
using CodeArt.Optimizely.HeadlessKit.Mvc;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
builder.Services.AddOptimizelyGraph(builder.Configuration);

var app = builder.Build();

app.UseCmsPreview();  // Must be before UseRouting
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapDynamicPageRoute<ContentRouteTransformer>("{**path}");
app.MapRazorPages();

await app.InitializeServicesAsync();  // Must be after routing setup

app.Run();
```

### Program.cs (MVC Controllers)

```csharp
using CodeArt.Optimizely.HeadlessKit.Mvc;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
builder.Services.AddOptimizelyGraph(builder.Configuration);

var app = builder.Build();

app.UseCmsPreview();  // Must be before UseRouting
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapContentControllerRoute();

await app.InitializeServicesAsync();  // Must be after routing setup

app.Run();
```

### _ViewImports.cshtml

```html
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, CodeArt.Optimizely.HeadlessKit
```

## Content Type Patterns

### Experience Page (Visual Builder)

```csharp
using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.ComponentModel.DataAnnotations;

[ContentType("StandardPage", BaseTypes.Experience, MayContainTypes = new[] {
    "HeroElement", "BannerElement", "EditorialElement", "CardElement"
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

### Element (Visual Builder Component)

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

    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string ButtonText { get; set; }
}
```

### Property Type Mapping

| .NET Type | CMS Behavior | Notes |
|-----------|-------------|-------|
| `string` | Text | Add `Format = PropertyFormats.ShortString` for single-line |
| `string` (no format) | Long text | Multi-line by default |
| `int` | Integer | |
| `bool` | Boolean | |
| `DateTime` | Date/time picker | |
| `float` / `double` | Decimal number | |
| `List<string>` | String array | Add `ItemsFormat = PropertyFormats.ShortString` |
| `GraphContentReference` | Content/image picker | Auto-detected type |
| `GraphContentRichText` | Rich text HTML editor | Auto-detected type |
| `GraphContentUrl` | URL picker | Auto-detected type |

### Dropdown/Enum Properties

```csharp
[CMSProperty(Format = PropertyFormats.SelectOne)]
[CMSPropertyChoice("left", "Left")]
[CMSPropertyChoice("center", "Center")]
[CMSPropertyChoice("right", "Right")]
public string Alignment { get; set; } = "center";
```

### Supported Data Annotations

`[Required]`, `[MaxLength(n)]`, `[MinLength(n)]`, `[StringLength(max)]`, `[Range(min, max)]`, `[RegularExpression(pattern)]`

## Page Template Patterns

### Razor Page Template

```csharp
// Pages/StandardPage.cshtml.cs
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using CodeArt.Optimizely.HeadlessKit.Mvc.Models;

[TemplateDescriptor(typeof(StandardPage))]
public class StandardPageModel : ContentPage<StandardPage> { }
```

```html
<!-- Pages/StandardPage.cshtml -->
@page
@model StandardPageModel
@{
    ViewData["Title"] = Model.CurrentContent?.MetaTitle;
}

<graph-content-area composition="@Model.CurrentContent?.Composition" />
```

### MVC Controller Template

For minimal content controllers, the base class handles everything -- just declare the class:

```csharp
// Controllers/StandardPageController.cs
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using CodeArt.Optimizely.HeadlessKit.Mvc.Controllers;

[TemplateDescriptor(typeof(StandardPage))]
public class StandardPageController : ContentControllerBase<StandardPage> { }
```

```html
<!-- Views/StandardPage/Index.cshtml -->
@model StandardPage
@{
    ViewData["Title"] = Model?.MetaTitle;
}

<graph-content-area composition="@Model?.Composition" />
```

Override `Index` when custom logic is needed:

```csharp
[TemplateDescriptor(typeof(ArticlePage))]
public class ArticlePageController : ContentControllerBase<ArticlePage>
{
    public override async Task<IActionResult> Index(ArticlePage CurrentContent)
    {
        // Custom logic...
        return View(CurrentContent);
    }
}
```

## Component Rendering Patterns

### Convention-Based (No ViewComponent Needed)

Place a Razor view at `Views/Shared/Components/DefaultContent/{TypeName}.cshtml` (strip "Element" suffix):

```html
<!-- Views/Shared/Components/DefaultContent/Banner.cshtml (for BannerElement) -->
@model BannerElement

<section class="banner">
    <graph-image content="@Model.Image" css-class="banner-bg" />
    <h2>@Model.Heading</h2>
    <graph-rich-text content="@Model.Body" />
    <graph-link content="@Model.Link" css-class="banner-cta">@Model.LinkText</graph-link>
</section>
```

### Custom ViewComponent (When You Need Logic)

```csharp
[TemplateDescriptor(typeof(ArticleListElement))]
public class ArticleListViewComponent : ViewComponent
{
    private readonly ContentGraphClient _graphClient;

    public ArticleListViewComponent(ContentGraphClient graphClient)
    {
        _graphClient = graphClient;
    }

    public async Task<IViewComponentResult> InvokeAsync(ArticleListElement model)
    {
        var articles = await GraphQuery.For<ArticlePage>(_graphClient)
            .Where(f => f.Metadata.Status.Eq("Published"))
            .OrderBy(a => a.MetaData.Published, OrderDirection.DESC)
            .Take(model.Count > 0 ? model.Count : 6)
            .ToListAsync();

        ViewBag.Heading = model.Heading;
        return View(articles);
    }
}
```

### Typed Base Class

```csharp
public class MyElementViewComponent : BlockViewComponentBase<MyElement>
{
    public override async Task<IViewComponentResult> InvokeAsync(MyElement model)
    {
        var related = await ContentRepository.GetContent<ArticlePage>(model.RelatedKey);
        ViewBag.Related = related;
        return View(model);
    }
}
```

## Query Patterns

### IContentRepository (Cached, Simple)

```csharp
// Inject IContentRepository
var page = await _repository.GetContentByPath<StandardPage>("/en/about");
var item = await _repository.GetContent<ArticlePage>("content-key");
var children = await _repository.GetChildren<ArticlePage>("parent-key");
```

### Fluent Query Builder

```csharp
using CodeArt.Optimizely.HeadlessKit.ContentClient;

// List with filters
var articles = await GraphQuery.For<ArticlePage>(client)
    .Where(f => f.Metadata.Status.Eq("Published"))
    .OrderBy(a => a.MetaData.Published, OrderDirection.DESC)
    .Take(10)
    .ToListAsync();

// Single item
var page = await GraphQuery.For<StandardPage>(client)
    .ForUrl("/en/about")
    .FirstOrDefaultAsync();

// Complex filters
var featured = await GraphQuery.For<ArticlePage>(client)
    .Where(f => f.And(
        f.Metadata.Status.Eq("Published"),
        f.Field("FeaturedImage").Exists(true),
        f.Or(
            f.Field("Category").Eq("news"),
            f.Field("Category").Eq("featured")
        )
    ))
    .Take(5)
    .ToListAsync();
```

### Search

```csharp
var results = await GraphQuery.SearchPages<ArticlePage>(client)
    .Fuzzy("search term")
    .Locale("en")
    .Facet("Tags")
    .Highlight("Body", fragmentSize: 200)
    .Take(20)
    .ExecuteAsync();

// results.Items, results.Total, results.Facets
```

### Pagination

```csharp
// Async enumerable (auto-pages)
await foreach (var article in GraphQuery.For<ArticlePage>(client)
    .Where(f => f.Metadata.Status.Eq("Published"))
    .ToAsyncEnumerable(pageSize: 20))
{
    // process each
}

// Cursor-based
var result = await GraphQuery.For<ArticlePage>(client)
    .Take(10).ToPagedResultAsync();
// result.Items, result.Total, result.Cursor, result.HasMore
```

## Tag Helper Usage

```html
<!-- Image -->
<graph-image content="@Model.Image" width="800" height="600" alt="Photo" css-class="img" />

<!-- Link -->
<graph-link content="@Model.Link" css-class="btn" target="_blank">Click</graph-link>

<!-- Rich text (raw HTML output) -->
<graph-rich-text content="@Model.Body" />

<!-- Content area (renders full composition) -->
<graph-content-area composition="@Model.CurrentContent?.Composition" css-class="main" />

<!-- CMS preview scripts (in _Layout.cshtml before </body>) -->
<cms-preview-scripts />
```

## Display Template Pattern

```csharp
using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

[DisplayTemplate(Key = "SectionDefault", DisplayName = "Section",
    BaseType = BaseTypes.Section, IsDefault = true)]
public class SectionDisplayTemplate : SaaSDisplayTemplate
{
    [JsonIgnore]
    [DisplayTemplateSetting(DisplayName = "Color Scheme", SortOrder = 10)]
    [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
    [DisplayTemplateChoice("dark", "Dark", SortOrder = 2)]
    [DisplayTemplateChoice("light", "Light", SortOrder = 3)]
    public string ColorScheme { get; set; } = "default";
}
```

## Sample Sites

Two complete sample sites are included in the repository:

- **`samples/HeadlessKit.Sample.RazorPages/`** -- Razor Pages approach using `ContentPage<T>` and `MapDynamicPageRoute`
- **`samples/HeadlessKit.Sample.Mvc/`** -- MVC Controllers approach using `ContentControllerBase<T>` and `MapContentControllerRoute`

Both sites use identical content types, element models, display templates, and CSS. They differ only in the rendering approach. Use these as reference implementations when building new sites.

## Common Mistakes to Avoid

1. **Missing `@addTagHelper *, CodeArt.Optimizely.HeadlessKit`** in `_ViewImports.cshtml` -- tag helpers won't work
2. **Forgetting `await app.InitializeServicesAsync()`** -- templates won't be discovered
3. **Not calling `app.UseCmsPreview()` before `app.UseRouting()`** -- preview won't work
4. **Using `BaseTypes.Page` instead of `BaseTypes.Experience`** for Visual Builder pages -- no composition support
5. **Using `GraphPageContent` instead of `GraphExperience`** as base class for experience pages
6. **Missing `[JsonIgnore]` on display template setting properties** -- properties get serialized to API
7. **Forgetting `MayContainTypes`** on experience pages -- no elements allowed in Visual Builder
8. **Not setting `Format = PropertyFormats.ShortString`** for short text -- defaults to long text editor
9. **Using `MapDynamicPageRoute` with MVC Controllers** -- use `MapContentControllerRoute` instead

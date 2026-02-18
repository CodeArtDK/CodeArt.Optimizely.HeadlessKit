# MVC Integration

HeadlessKit provides ASP.NET Core integration for content routing, rendering, and CMS preview support. Both Razor Pages and MVC Controllers are fully supported -- see the sample sites at `samples/HeadlessKit.Sample.RazorPages/` and `samples/HeadlessKit.Sample.Mvc/` for complete working examples of each approach.

## Content Routing

The `ContentRouteTransformer` intercepts requests via a catch-all dynamic route, fetches content from Optimizely Graph, and maps it to the appropriate Razor Page or MVC Controller.

### Setup

```csharp
// In Program.cs

// For Razor Pages:
app.MapDynamicPageRoute<ContentRouteTransformer>("{**path}");

// Or for MVC Controllers:
app.MapContentControllerRoute();

// Initialize template scanning
await app.InitializeServicesAsync();
```

### Request Flow

```
HTTP Request (/en/about)
  |
  v
CmsPreviewMiddleware (sets CSP headers if /preview)
  |
  v
ContentRouteTransformer
  |- Queries Graph: GetContentByPath("/en/about")
  |- Gets typed content (e.g., StandardPage)
  |- Stores in HttpContext.Items["CurrentContent"]
  |- Resolves template via TemplateCoordinator
  |
  v
Razor Page or Controller
  |- Model binding: CurrentContent from HttpContext
  |
  v
Razor View
  |- Tag helpers render content
  |- <graph-content-area> renders composition
```

### Preview Requests

The CMS sends preview requests to `/preview` with query parameters:
- `key` -- Content key
- `ver` -- Content version
- `preview_token` -- Authentication token
- `loc` -- Locale
- `ctx` -- Preview context (`"preview"` or `"edit"`)

The router handles these automatically, fetching the preview content with Bearer token auth.

## Razor Pages

### ContentPage Base Class

`ContentPage<T>` is the base `PageModel` for content-routed pages. It provides a `CurrentContent` property automatically bound from the routed content.

```csharp
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using CodeArt.Optimizely.HeadlessKit.Mvc.Models;

[TemplateDescriptor(typeof(StandardPage))]
public class StandardPageModel : ContentPage<StandardPage> { }
```

The Razor view accesses content via `Model.CurrentContent`:

```html
@page
@model StandardPageModel
@{
    ViewData["Title"] = Model.CurrentContent?.MetaTitle;
}

<h1>@Model.CurrentContent?.MetaTitle</h1>
<graph-content-area composition="@Model.CurrentContent?.Composition" />
```

## MVC Controllers

### ContentControllerBase

For MVC controller-based rendering:

```csharp
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using CodeArt.Optimizely.HeadlessKit.Mvc.Controllers;

[TemplateDescriptor(typeof(ArticlePage))]
public class ArticleController : ContentControllerBase<ArticlePage>
{
    public override async Task<IActionResult> Index(ArticlePage CurrentContent)
    {
        // Custom logic...
        return View(CurrentContent);
    }
}
```

## TemplateDescriptorAttribute

Registers a Razor Page, Controller, or ViewComponent as the rendering template for a content type:

```csharp
[TemplateDescriptor(typeof(StandardPage))]
```

| Property | Default | Description |
|----------|---------|-------------|
| `TemplateFor` | *(required)* | The content type this template renders |
| `RenderingTag` | `null` | Optional alternate template identifier |
| `AvailableWithoutTag` | `true` | Can be used when no rendering tag is specified |
| `Inherited` | `true` | Also handles derived content types |

## Tag Helpers

Add to `_ViewImports.cshtml`:

```html
@addTagHelper *, CodeArt.Optimizely.HeadlessKit
```

### graph-image

Renders a `GraphContentReference` as an `<img>` element:

```html
<graph-image content="@Model.CurrentContent.FeaturedImage"
             width="800" height="600"
             alt="Featured image"
             css-class="article-image" />
```

| Attribute | Type | Description |
|-----------|------|-------------|
| `content` | `GraphContentReference` | Image reference with URL |
| `width` | `int?` | Image width |
| `height` | `int?` | Image height |
| `alt` | `string` | Alt text |
| `css-class` | `string` | CSS class |

Suppresses output if the image URL is null.

### graph-link

Renders a `GraphContentUrl` as an `<a>` element:

```html
<graph-link content="@Model.Link" css-class="btn" target="_blank">
    Click here
</graph-link>
```

| Attribute | Type | Description |
|-----------|------|-------------|
| `content` | `GraphContentUrl` | Link URL |
| `css-class` | `string` | CSS class |
| `target` | `string` | Link target |

If the URL is null, renders only the inner content without a wrapper.

### graph-rich-text

Renders a `GraphContentRichText` as raw HTML:

```html
<graph-rich-text content="@Model.Body" />
```

Replaces the tag element with the HTML content directly.

### graph-content-area

Renders a `ContentComposition` by invoking the composition rendering pipeline:

```html
<graph-content-area composition="@Model.CurrentContent?.Composition"
                     css-class="main-content" />
```

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `composition` | `ContentComposition` | | Composition tree to render |
| `css-class` | `string` | `"opti-content-area"` | Wrapper CSS class |

### cms-preview-scripts

Injects CMS communication scripts for preview/edit mode:

```html
<!-- In _Layout.cshtml, before </body> -->
<cms-preview-scripts />
```

Only renders when:
1. The request is in preview mode
2. `OptimizelyGraph:CmsAppUrl` is configured

Outputs the CMS communication injector script and a content-saved event handler that reloads the preview.

## Composition Rendering

The composition rendering pipeline turns a `ContentComposition` tree into HTML:

### Pipeline

1. `<graph-content-area>` invokes `CompositionRendererViewComponent`
2. Renderer iterates top-level nodes, invoking `CompositionNodeViewComponent` for each
3. **Structure nodes** (Section, Row, Column) render a wrapper `<div>` with CSS classes and data attributes, then recurse into child nodes
4. **Component nodes** look up the registered ViewComponent via `TemplateCoordinator`, and invoke it with the content model
5. If no custom ViewComponent is found, `DefaultContentViewComponent` looks for a view by convention

### HTML Output

```html
<div class="opti-content-area">
    <div class="opti-section opti-dark opti-spacious"
         data-node-type="section"
         data-display-template="SectionDefault"
         data-display-colorScheme="dark"
         data-display-padding="spacious">
        <div class="opti-row">
            <div class="opti-component opti-fullWidth"
                 data-display-template="HeroElementDefault">
                <!-- ViewComponent output for HeroElement -->
            </div>
        </div>
    </div>
</div>
```

### Display Settings Resolution

The `DefaultDisplaySettingsResolver` converts display settings to:
- **CSS classes:** `opti-{value}` (e.g., `opti-dark`, `opti-spacious`)
- **Data attributes:** `data-display-{key}="{value}"`

Replace with a custom `IDisplaySettingsResolver` via DI for different CSS naming.

## View Components

### Custom ViewComponent

Register a ViewComponent for a content type with `[TemplateDescriptor]`:

```csharp
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using Microsoft.AspNetCore.Mvc;

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

### BlockViewComponentBase

A typed base class with `IContentRepository` access:

```csharp
public class MyElementViewComponent : BlockViewComponentBase<MyElement>
{
    public override async Task<IViewComponentResult> InvokeAsync(MyElement model)
    {
        // ContentRepository is available for additional queries
        var related = await ContentRepository.GetContent<ArticlePage>(model.RelatedKey);
        ViewBag.Related = related;
        return View(model);
    }
}
```

### DefaultContentViewComponent

When no custom ViewComponent is registered for a content type, the `DefaultContentViewComponent` looks for a Razor view by convention:

```
Views/Shared/Components/DefaultContent/{TypeName}.cshtml
```

The "Element" suffix is stripped from the type name. For example, `BannerElement` resolves to `Components/DefaultContent/Banner.cshtml`:

```html
@model BannerElement

<section class="banner">
    <graph-image content="@Model.Image" css-class="banner-bg" />
    <div class="banner-content">
        <h2>@Model.Heading</h2>
        <graph-rich-text content="@Model.Body" />
        <graph-link content="@Model.Link" css-class="banner-cta">
            @Model.LinkText
        </graph-link>
    </div>
</section>
```

### InvokeGraphContentComponentAsync

Extension method for manually invoking content components in views:

```html
@await Component.InvokeGraphContentComponentAsync(myContentItem)
@await Component.InvokeGraphContentComponentAsync(myContentItem, renderingTag: "sidebar")
```

## Preview Support

### CmsPreviewMiddleware

Handles iframe security for CMS preview. Must be added before `UseRouting()`:

```csharp
app.UseCmsPreview();
```

On `/preview` requests, it:
- Removes `X-Frame-Options` header
- Adds `Content-Security-Policy: frame-ancestors 'self' {CmsAppUrl}` to allow CMS iframe embedding

### Detecting Preview Mode

```csharp
// In a Razor Page or Controller:
if (HttpContext.IsPreview())
{
    // In preview mode
}

if (HttpContext.IsOnPageEdit())
{
    // In on-page edit mode
}
```

### Configuration

Set `OptimizelyGraph:CmsAppUrl` to your CMS application URL:

```json
{
  "OptimizelyGraph": {
    "CmsAppUrl": "https://app-xxxxx.cms.optimizely.com"
  }
}
```

# CodeArt.Optimizely.HeadlessKit

.NET 10 library for headless Optimizely CMS (SaaS). Single NuGet package combining content type definition, GraphQL content querying, and ASP.NET Core MVC rendering.

## Namespace

`CodeArt.Optimizely.HeadlessKit` -- all types are under this root namespace.

## Service Registration

```csharp
builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);  // TypeBuilder (syncs types to CMS)
builder.Services.AddOptimizelyGraph(builder.Configuration);      // ContentClient + MVC integration
```

## Configuration Sections

- `SaaSCMS` -- ClientId, ClientSecret, SyncOnStartup, etc.
- `OptimizelyGraph` -- SingleKey, GraphEndpoint, CmsAppUrl, CacheDurationSeconds

## Content Types

POCOs with `[ContentType]` attribute. Experience pages inherit `GraphExperience`, elements inherit `GraphBlock`.

```csharp
[ContentType("HeroElement", BaseTypes.Element)]
public class HeroElement : GraphBlock
{
    [CultureSpecific]
    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string Title { get; set; }

    public GraphContentReference Image { get; set; }
    public GraphContentRichText Body { get; set; }
    public GraphContentUrl Link { get; set; }
}
```

## Page Templates

Razor Pages: inherit `ContentPage<T>`, annotate with `[TemplateDescriptor(typeof(T))]`.
Controllers: inherit `ContentControllerBase<T>`, annotate with `[TemplateDescriptor(typeof(T))]`.

## Content Routing

```csharp
app.UseCmsPreview();

// For Razor Pages:
app.MapDynamicPageRoute<ContentRouteTransformer>("{**path}");

// Or for MVC Controllers:
app.MapContentControllerRoute();

await app.InitializeServicesAsync();
```

## Querying

```csharp
// Repository (cached)
var page = await contentRepository.GetContentByPath<StandardPage>("/en/about");

// Fluent builder
var articles = await GraphQuery.For<ArticlePage>(client)
    .Where(f => f.Metadata.Status.Eq("Published"))
    .OrderBy(a => a.MetaData.Published, OrderDirection.DESC)
    .Take(10)
    .ToListAsync();

// Search
var results = await GraphQuery.SearchPages<ArticlePage>(client)
    .Fuzzy("search term")
    .ExecuteAsync();
```

## Tag Helpers

```html
@addTagHelper *, CodeArt.Optimizely.HeadlessKit

<graph-image content="@Model.Image" width="800" alt="..." css-class="..." />
<graph-link content="@Model.Link" css-class="btn">Link text</graph-link>
<graph-rich-text content="@Model.Body" />
<graph-content-area composition="@Model.Composition" css-class="main" />
<cms-preview-scripts />
```

## View Components

- Custom: `[TemplateDescriptor(typeof(MyElement))]` on a `ViewComponent` class
- Convention: `Views/Shared/Components/DefaultContent/{TypeName}.cshtml` (strips "Element" suffix)
- Base class: `BlockViewComponentBase<T>` with `IContentRepository`

## Display Templates

```csharp
[DisplayTemplate(Key = "SectionDefault", DisplayName = "Section",
    BaseType = BaseTypes.Section, IsDefault = true)]
public class SectionDisplayTemplate : SaaSDisplayTemplate
{
    [JsonIgnore]
    [DisplayTemplateSetting(DisplayName = "Color Scheme", SortOrder = 10)]
    [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
    [DisplayTemplateChoice("dark", "Dark", SortOrder = 2)]
    public string ColorScheme { get; set; } = "default";
}
```

## Key Types

| Purpose | Type |
|---------|------|
| Page base | `GraphExperience` (Visual Builder) or `GraphPageContent` (traditional) |
| Element/block base | `GraphBlock` |
| Image reference | `GraphContentReference` |
| Rich text | `GraphContentRichText` |
| URL | `GraphContentUrl` |
| Metadata | `GraphContentMetadata` |
| Composition | `ContentComposition` |
| Repository | `IContentRepository` |
| Graph client | `ContentGraphClient` |
| Query builder | `GraphQuery.For<T>()` |
| Search builder | `GraphQuery.SearchPages<T>()` |

# Content Querying

HeadlessKit provides two ways to query content from Optimizely Graph:
- **IContentRepository** -- Simple, cached queries by path, key, or children
- **GraphQueryBuilder / SearchQueryBuilder** -- Fluent builders for custom queries with filters, sorting, and pagination

## IContentRepository (Simple Queries)

Inject `IContentRepository` for basic cached content retrieval:

```csharp
public class MyService
{
    private readonly IContentRepository _repository;

    public MyService(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task Example()
    {
        // Get content by URL path
        var page = await _repository.GetContentByPath<StandardPage>("/en/about");

        // Get content by key
        var content = await _repository.GetContent<ArticlePage>("abc-123-def");

        // Get child content
        var children = await _repository.GetChildren<ArticlePage>("parent-key-123");
    }
}
```

Results are cached in-memory. Cache duration is configured via `OptimizelyGraph:CacheDurationSeconds` (default 300 seconds, 0 to disable).

## ContentGraphClient (Direct Queries)

For more control, inject `ContentGraphClient` directly:

```csharp
// Get by path
var page = await client.GetContentByPath<StandardPage>("/en/about");

// Get by key
var page = await client.GetContentByKey<StandardPage>("abc-123");

// Get children
var children = await client.GetChildren<ArticlePage>("parent-key");

// Preview content (CMS preview mode)
var preview = await client.GetPreviewContentByKey<StandardPage>(
    key: "abc-123",
    version: "draft",
    previewToken: "token-from-cms");

// Execute raw GraphQL
var result = await client.ExecuteQueryAsync<ArticlePage>(
    "{ ArticlePage { items { Title } } }");
```

## Fluent Query Builder

`GraphQuery.For<T>()` creates a fluent builder for typed queries:

```csharp
using CodeArt.Optimizely.HeadlessKit.ContentClient;

// Basic query
var articles = await GraphQuery.For<ArticlePage>(client)
    .Where(f => f.Metadata.Status.Eq("Published"))
    .OrderBy(a => a.MetaData.Published, OrderDirection.DESC)
    .Take(10)
    .ToListAsync();

// Single item
var page = await GraphQuery.For<StandardPage>(client)
    .ForUrl("/en/about")
    .FirstOrDefaultAsync();

// With composition data
var experience = await GraphQuery.For<StandardPage>(client)
    .ForKey("abc-123")
    .WithComposition(depth: 3)
    .FirstOrDefaultAsync();
```

### Builder Methods

| Method | Description |
|--------|-------------|
| `Where(f => ...)` | Add filter expression |
| `Where(filter)` | Add pre-built GraphFilter |
| `Locale("en", "sv")` | Set query locale(s) |
| `Skip(n)` | Skip n items (pagination) |
| `Take(n)` | Limit to n items |
| `OrderBy(field, direction)` | Order by field path string |
| `OrderBy(x => x.Prop, dir)` | Order by property expression |
| `WithComposition(depth)` | Include composition data |
| `ForUrl(url)` | Filter by URL path |
| `ForKey(key)` | Filter by content key |
| `ForType(typeName)` | Override GraphQL type name |
| `After(cursor)` | Cursor-based pagination |

### Execution Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ToListAsync()` | `List<T>` | Execute and return items |
| `FirstOrDefaultAsync()` | `T?` | Execute with Take(1), return first or null |
| `ExecuteAsync()` | `GraphQueryResult<T>` | Full result with items, total, cursor |
| `ToPagedResultAsync()` | `PagedResult<T>` | Items + Total + Cursor + HasMore |
| `ToAsyncEnumerable(pageSize)` | `IAsyncEnumerable<T>` | Auto-paging async stream |
| `Build()` | `string` | Return the generated GraphQL query |

### Pagination

```csharp
// Offset pagination
var page1 = await GraphQuery.For<ArticlePage>(client)
    .Skip(0).Take(10).ExecuteAsync();

// Cursor-based pagination
var result = await GraphQuery.For<ArticlePage>(client)
    .Take(10).ToPagedResultAsync();

while (result.HasMore)
{
    // process result.Items...
    result = await GraphQuery.For<ArticlePage>(client)
        .Take(10).After(result.Cursor).ToPagedResultAsync();
}

// Async enumerable (auto-pages)
await foreach (var article in GraphQuery.For<ArticlePage>(client)
    .Where(f => f.Metadata.Status.Eq("Published"))
    .ToAsyncEnumerable(pageSize: 20))
{
    // process each article
}
```

## GraphFilter System

Filters are built using a fluent expression in `Where()`:

```csharp
.Where(f => f.Metadata.Status.Eq("Published"))
```

### Field Filters

| Method | Description | Example |
|--------|-------------|---------|
| `Eq(value)` | Equals | `f.Field("Title").Eq("Hello")` |
| `NotEq(value)` | Not equals | `f.Field("Status").NotEq("Draft")` |
| `Like(pattern)` | Wildcard match | `f.Field("Title").Like("*news*")` |
| `StartsWith(value)` | Starts with | `f.Field("Title").StartsWith("Breaking")` |
| `EndsWith(value)` | Ends with | `f.Field("Slug").EndsWith("-2024")` |
| `Contains(value)` | Contains substring | `f.Field("Title").Contains("important")` |
| `Exists(bool)` | Has/doesn't have value | `f.Field("Image").Exists(true)` |
| `In(values)` | In list | `f.Field("Category").In("news", "blog")` |
| `NotIn(values)` | Not in list | `f.Field("Status").NotIn("Draft", "Archived")` |
| `Gt(value)` | Greater than | `f.Field("Views").Gt(100)` |
| `Gte(value)` | Greater or equal | `f.Field("Rating").Gte(4)` |
| `Lt(value)` | Less than | `f.Field("Price").Lt(50)` |
| `Lte(value)` | Less or equal | `f.Field("Stock").Lte(10)` |

### Metadata Filters

```csharp
// Filter by content key
.Where(f => f.Metadata.Key.Eq("abc-123"))

// Filter by URL
.Where(f => f.Metadata.Url.Default.Eq("/en/about"))

// Filter by status
.Where(f => f.Metadata.Status.Eq("Published"))

// Filter by type
.Where(f => f.Metadata.Types.In("ArticlePage", "BlogPage"))

// Filter by locale
.Where(f => f.Metadata.Locale.Eq("en"))
```

### Boolean Composition

```csharp
// AND (all conditions must match)
.Where(f => f.And(
    f.Metadata.Status.Eq("Published"),
    f.Field("Category").Eq("news"),
    f.Field("FeaturedImage").Exists(true)
))

// OR (any condition matches)
.Where(f => f.Or(
    f.Field("Category").Eq("news"),
    f.Field("Category").Eq("blog")
))

// NOT (negate a condition)
.Where(f => f.Not(f.Field("Status").Eq("Draft")))

// Complex combinations
.Where(f => f.And(
    f.Metadata.Status.Eq("Published"),
    f.Or(
        f.Field("Category").Eq("news"),
        f.Field("Category").Eq("blog")
    ),
    f.Not(f.Field("Archived").Eq(true))
))
```

### Full-Text Filters

```csharp
.Where(f => f.Fulltext.Match("search terms"))
.Where(f => f.Fulltext.Contains("partial text"))
.Where(f => f.Fulltext.Fuzzy("aproximate"))  // tolerates typos
.Where(f => f.Fulltext.Boost("important term", boost: 10))
```

## Search Query Builder

For full-text search with facets and highlights:

```csharp
var results = await GraphQuery.SearchPages<ArticlePage>(client)
    .Fuzzy("optimizely headless")
    .Locale("en")
    .Facet("Tags")
    .Highlight("Body", fragmentSize: 200)
    .OrderBy("_score", OrderDirection.DESC)
    .Take(20)
    .ExecuteAsync();

// results.Items     -- matched articles
// results.Total     -- total count
// results.Facets    -- aggregated facet values
```

### Search Methods

| Method | Description |
|--------|-------------|
| `Match(text)` | Exact full-text search |
| `Fuzzy(text)` | Fuzzy search (tolerates typos) |
| `Boost(weight)` | Relevance boost factor |
| `Facet(field)` | Add facet aggregation |
| `Highlight(field, fragmentSize)` | Add result highlighting |

## Content Type Registry

The `IContentTypeRegistry` discovers content types at startup by scanning assemblies for `IGraphContent` implementations:

```csharp
var registry = serviceProvider.GetRequiredService<IContentTypeRegistry>();

// All registered page types
IReadOnlyCollection<Type> pageTypes = registry.PageTypes;

// All component types
IReadOnlyCollection<Type> componentTypes = registry.ComponentTypes;

// Resolve type by name
Type? articleType = registry.ResolveType("ArticlePage");
```

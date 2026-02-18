# Content Types

Content types map .NET POCOs to Optimizely CMS SaaS content type definitions. The TypeBuilder discovers annotated classes at startup, compares them with the remote CMS API, and creates or updates types as needed. It never deletes types.

## ContentTypeAttribute

Apply `[ContentType]` to a class to register it as a CMS content type.

```csharp
[ContentType("StandardPage", BaseTypes.Experience,
    DisplayName = "Standard Page",
    Description = "A general-purpose page",
    MayContainTypes = new[] { "HeroElement", "BannerElement" })]
public class StandardPage : GraphExperience
{
    // properties...
}
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Key` | string | *(required)* | Unique CMS identifier for the type |
| `BaseType` | BaseTypes | *(required)* | The base content type (see below) |
| `DisplayName` | string | `null` | Label shown in CMS editor. Defaults to class name if not set. |
| `Description` | string | `null` | Description shown in CMS editor |
| `SortOrder` | int | `0` | Sort order in editor type lists |
| `Features` | Features | Versioning \| Localization \| PublishPeriod | Enabled features |
| `Usages` | Usages | Property \| Instance | How the type can be used |
| `MayContainTypes` | string[] | `null` | Keys of allowed child content types |
| `MediaFileExtensions` | string[] | `null` | Allowed file extensions (media types only) |
| `CompositionBehaviors` | string[] | `null` | Composition behavior overrides |
| `Contracts` | string[] | `null` | Contract keys this type implements |
| `IsContract` | bool | `false` | Whether this is a contract type |
| `Source` | string | `null` | Source identifier |

## Base Types

| BaseTypes Value | Base Class | Description |
|----------------|------------|-------------|
| `BaseTypes.Experience` | `GraphExperience` | Visual Builder page with composition support |
| `BaseTypes.Page` | `GraphPageContent` | Traditional routable page |
| `BaseTypes.Element` | `GraphBlock` | Visual Builder component/element |
| `BaseTypes.Block` | `GraphBlock` | Reusable content block |
| `BaseTypes.Section` | *(structural)* | Visual Builder layout section |
| `BaseTypes.Media` | `GraphMedia` | Generic media asset |
| `BaseTypes.Image` | `GraphImage` | Image media |
| `BaseTypes.Video` | `GraphVideo` | Video media |
| `BaseTypes.Folder` | *(structural)* | Content organizer |

### Experience Pages vs Traditional Pages

For sites using Optimizely's Visual Builder, use `BaseTypes.Experience` and inherit from `GraphExperience`. This gives you a `Composition` property containing the page's visual layout tree.

For traditional CMS pages without composition, use `BaseTypes.Page` and inherit from `GraphPageContent`.

## CMSPropertyAttribute

Apply `[CMSProperty]` to properties to configure their CMS editor behavior.

```csharp
[CMSProperty(
    DisplayName = "Title",
    Description = "The page title",
    Group = "Content",
    SortOrder = 10,
    Format = PropertyFormats.ShortString)]
public string Title { get; set; }
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DisplayName` | string | `null` | Label in CMS editor |
| `Description` | string | `null` | Help text in editor |
| `Group` | string | `null` | Property group name for editor organization |
| `SortOrder` | int | `0` | Order within the group |
| `Type` | string | `null` | CMS property type (usually auto-detected) |
| `Format` | string | `null` | Property format (see PropertyFormats) |
| `IndexingType` | string | `null` | Graph indexing configuration |
| `AllowedTypes` | string[] | `null` | Allowed content type keys (for references) |
| `RestrictedTypes` | string[] | `null` | Restricted content type keys |
| `ContentType` | string | `null` | Specific content type for reference properties |
| `ItemsType` | string | `null` | Item type for array properties |
| `ItemsFormat` | string | `null` | Item format for array properties |
| `ItemsAllowedTypes` | string[] | `null` | Allowed types for array items |
| `ItemsRestrictedTypes` | string[] | `null` | Restricted types for array items |

## Property Type Mapping

The TypeBuilder maps .NET property types to CMS types automatically:

| .NET Type | CMS Type | Notes |
|-----------|----------|-------|
| `string` | `string` | Use `Format = PropertyFormats.ShortString` for single-line text |
| `int` | `integer` | |
| `bool` | `boolean` | |
| `DateTime` | `dateTime` | |
| `float` / `double` | `float` | |
| `List<string>` | `array` | Use `ItemsFormat = PropertyFormats.ShortString` for string lists |
| `GraphContentReference` | `contentReference` | Reference to another content item (image, page, etc.) |
| `GraphContentRichText` | `richText` | Rich text HTML content |
| `GraphContentUrl` | `url` | URL with multiple representations |

## PropertyFormats Constants

Use `PropertyFormats` constants instead of raw strings:

| Constant | Value | CMS Editor |
|----------|-------|------------|
| `PropertyFormats.ShortString` | `"shortString"` | Single-line text input |
| `PropertyFormats.SelectOne` | `"selectOne"` | Dropdown selection (use with `[CMSPropertyChoice]`) |
| `PropertyFormats.ListOfString` | `"listOfString"` | Multi-value string list |
| `PropertyFormats.ImageUrl` | `"imageUrl"` | Image picker |
| `PropertyFormats.DocumentUrl` | `"documentUrl"` | Document/file picker |
| `PropertyFormats.Html` | `"html"` | Rich text HTML editor |

## CultureSpecificAttribute

Mark properties as localizable (translatable per language):

```csharp
[CultureSpecific]
[CMSProperty(Format = PropertyFormats.ShortString)]
public string Title { get; set; }
```

## CMSPropertyChoiceAttribute

Define selectable values for `SelectOne` properties:

```csharp
[CMSProperty(Format = PropertyFormats.SelectOne)]
[CMSPropertyChoice("left", "Left")]
[CMSPropertyChoice("center", "Center")]
[CMSPropertyChoice("right", "Right")]
public string Alignment { get; set; } = "left";
```

## CMSDateRangeAttribute

Restrict DateTime properties to a date range:

```csharp
[CMSDateRange("2020-01-01", "2030-12-31")]
public DateTime EventDate { get; set; }
```

## Data Annotation Support

Standard .NET data annotations are mapped to CMS validation:

| Annotation | CMS Effect |
|------------|------------|
| `[Required]` | Property marked as required |
| `[MaxLength(n)]` | Maximum string length |
| `[MinLength(n)]` | Minimum string length |
| `[StringLength(max, MinimumLength = min)]` | Min/max string length |
| `[Range(min, max)]` | Numeric range validation |
| `[RegularExpression(pattern)]` | Regex pattern validation |

## MayContainTypes

For Experience pages, specify which element types can be used in the composition:

```csharp
[ContentType("StandardPage", BaseTypes.Experience, MayContainTypes = new[] {
    "HeroElement", "BannerElement", "EditorialElement", "CardElement"
})]
public class StandardPage : GraphExperience { }
```

## Complete Example

```csharp
using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.ComponentModel.DataAnnotations;

[ContentType("ArticlePage", BaseTypes.Experience,
    DisplayName = "Article Page",
    MayContainTypes = new[] { "HeroElement", "EditorialElement", "ImageElement" })]
public class ArticlePage : GraphExperience
{
    [CultureSpecific]
    [MaxLength(200)]
    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string MetaTitle { get; set; }

    [CultureSpecific]
    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string Title { get; set; }

    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string Excerpt { get; set; }

    public GraphContentReference FeaturedImage { get; set; }

    public GraphContentRichText Body { get; set; }

    public DateTime PublishedDate { get; set; }

    [CMSProperty(Format = PropertyFormats.ShortString)]
    public string AuthorName { get; set; }

    public GraphContentReference AuthorPhoto { get; set; }

    [MaxLength(20)]
    [CMSProperty(ItemsFormat = PropertyFormats.ShortString)]
    public List<string> Tags { get; set; }
}
```

## Type Sync Behavior

When the application starts (if `SyncOnStartup` is enabled):

1. **Assembly scanning** discovers all classes with `[ContentType]` and `[DisplayTemplate]`.
2. **Topological sort** orders types so dependencies (`MayContainTypes`) are synced before dependents.
3. **Comparison** checks each local type against the remote CMS API.
4. **Create** new types that don't exist remotely.
5. **Update** changed types if `UpdateExistingContentTypes` is enabled. Properties are merged additively -- remote-only properties are preserved.
6. **Never delete** -- types and properties that exist remotely but not locally are left untouched.

The sync produces a `SyncReport` with counts of created, updated, unchanged, and failed types, logged at startup.

# Display Templates

Display templates configure how content elements appear in the Optimizely CMS Visual Builder. They define editor-selectable settings (like color scheme, layout, padding) that are applied at render time as CSS classes and HTML data attributes.

## Overview

A display template is a class that:
1. Inherits from `SaaSDisplayTemplate`
2. Is annotated with `[DisplayTemplate]`
3. Has properties annotated with `[DisplayTemplateSetting]` and `[DisplayTemplateChoice]`

Display templates are synced to the CMS alongside content types on startup.

## DisplayTemplateAttribute

```csharp
[DisplayTemplate(Key = "SectionDefault", DisplayName = "Section",
    BaseType = BaseTypes.Section, IsDefault = true)]
public class SectionDisplayTemplate : SaaSDisplayTemplate { }
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Key` | string | `null` | Unique template identifier |
| `DisplayName` | string | `null` | Label shown in CMS editor |
| `BaseType` | BaseTypes | *(unset)* | Target base type (Section, Experience, Element) |
| `ContentType` | string | `null` | Specific content type key this applies to |
| `NodeType` | string | `null` | Layout element type (e.g., "Section", "Row") |
| `IsDefault` | bool | `false` | Default template for the type |

Set either `BaseType` (applies to all types of that base) or `ContentType` (applies to a specific type), not both.

## DisplayTemplateSettingAttribute

Exposes a property as an editor-configurable setting:

```csharp
[DisplayTemplateSetting(DisplayName = "Color Scheme", SortOrder = 10)]
public string ColorScheme { get; set; } = "default";
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DisplayName` | string | `null` | Setting label in CMS editor |
| `Editor` | string | `"choice"` | Editor type |
| `SortOrder` | int | `0` | Order in editor UI |

## DisplayTemplateChoiceAttribute

Defines selectable options for a setting. Apply multiple times on the same property:

```csharp
[DisplayTemplateChoice("default", "Default", SortOrder = 1)]
[DisplayTemplateChoice("dark", "Dark", SortOrder = 2)]
[DisplayTemplateChoice("light", "Light", SortOrder = 3)]
public string ColorScheme { get; set; } = "default";
```

| Parameter/Property | Type | Description |
|-------------------|------|-------------|
| `value` (constructor) | string | Stored value |
| `displayName` (constructor) | string | Label in CMS editor |
| `SortOrder` | int | Order of the choice in the dropdown |

## Complete Example: Section Display Template

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
    [DisplayTemplateChoice("accent", "Accent", SortOrder = 4)]
    public string ColorScheme { get; set; } = "default";

    [JsonIgnore]
    [DisplayTemplateSetting(DisplayName = "Padding", SortOrder = 20)]
    [DisplayTemplateChoice("default", "Default", SortOrder = 1)]
    [DisplayTemplateChoice("compact", "Compact", SortOrder = 2)]
    [DisplayTemplateChoice("spacious", "Spacious", SortOrder = 3)]
    [DisplayTemplateChoice("none", "None", SortOrder = 4)]
    public string Padding { get; set; } = "default";
}
```

> **Note:** Properties should be marked `[JsonIgnore]` so they aren't serialized to the CMS API. Only the metadata (settings/choices) is used for sync.

## Complete Example: Content-Specific Display Template

```csharp
[DisplayTemplate(Key = "HeroElementDefault", DisplayName = "Hero Element",
    ContentType = "HeroElement", IsDefault = true)]
public class HeroElementDisplayTemplate : SaaSDisplayTemplate
{
    [JsonIgnore]
    [DisplayTemplateSetting(DisplayName = "Layout", SortOrder = 10)]
    [DisplayTemplateChoice("fullWidth", "Full Width", SortOrder = 1)]
    [DisplayTemplateChoice("contained", "Contained", SortOrder = 2)]
    [DisplayTemplateChoice("split", "Split", SortOrder = 3)]
    public string Layout { get; set; } = "fullWidth";

    [JsonIgnore]
    [DisplayTemplateSetting(DisplayName = "Height", SortOrder = 20)]
    [DisplayTemplateChoice("large", "Large", SortOrder = 1)]
    [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
    [DisplayTemplateChoice("small", "Small", SortOrder = 3)]
    public string Height { get; set; } = "large";

    [JsonIgnore]
    [DisplayTemplateSetting(DisplayName = "Text Alignment", SortOrder = 30)]
    [DisplayTemplateChoice("left", "Left", SortOrder = 1)]
    [DisplayTemplateChoice("center", "Center", SortOrder = 2)]
    [DisplayTemplateChoice("right", "Right", SortOrder = 3)]
    public string TextAlignment { get; set; } = "left";

    [JsonIgnore]
    [DisplayTemplateSetting(DisplayName = "Overlay Opacity", SortOrder = 40)]
    [DisplayTemplateChoice("dark", "Dark", SortOrder = 1)]
    [DisplayTemplateChoice("medium", "Medium", SortOrder = 2)]
    [DisplayTemplateChoice("light", "Light", SortOrder = 3)]
    [DisplayTemplateChoice("none", "None", SortOrder = 4)]
    public string OverlayOpacity { get; set; } = "dark";
}
```

## Runtime Rendering

At render time, display template settings are converted to CSS classes and HTML data attributes by `IDisplaySettingsResolver`.

### Default Behavior

The `DefaultDisplaySettingsResolver` generates:
- **CSS classes:** `opti-{value}` for each setting (e.g., `opti-dark`, `opti-spacious`)
- **Data attributes:** `data-display-{key}="{value}"` for each setting

For example, a section with `ColorScheme = "dark"` and `Padding = "spacious"` renders:

```html
<div class="opti-section opti-dark opti-spacious"
     data-display-template="SectionDefault"
     data-display-colorScheme="dark"
     data-display-padding="spacious">
    <!-- child nodes -->
</div>
```

### Custom Display Settings

Replace the default resolver by registering a custom `IDisplaySettingsResolver`:

```csharp
services.AddSingleton<IDisplaySettingsResolver, MyCustomResolver>();
```

### Using Display Templates in Views

The `ViewBag.DisplayTemplate` property contains the resolved template instance:

```html
@{
    var template = ViewBag.DisplayTemplate as HeroElementDisplayTemplate;
    var layout = template?.Layout ?? "fullWidth";
}

<section class="hero hero--@layout">
    <!-- render based on template settings -->
</section>
```

To enable `ViewBag.DisplayTemplate` resolution, the `IDisplayTemplateResolver` is automatically registered when you call `AddSaaSCMSTypeBuilder()`.

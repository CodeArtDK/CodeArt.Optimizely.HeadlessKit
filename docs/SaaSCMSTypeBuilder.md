# SaaS CMS Type Builder

> **Note:** This document has been superseded by the comprehensive documentation guides:
> - [Getting Started](getting-started.md) -- Setup and first steps
> - [Content Types](content-types.md) -- Full attribute reference and type sync behavior
> - [Display Templates](display-templates.md) -- Editor display settings

## Quick Reference

This library lets .NET applications define Optimizely CMS (SaaS) content types as POCOs with attributes. On startup, it scans the application assemblies, compares the discovered definitions with the SaaS CMS API, and creates or updates any missing content types and display templates without deleting anything.

### Configuration

Configure connection settings via the `SaaSCMS` section:

```json
{
  "SaaSCMS": {
    "ApiBaseUrl": "https://api.cms.optimizely.com/",
    "ApiPathPrefix": "preview3",
    "TokenEndpoint": "/oauth/token",
    "ClientId": "<client-id>",
    "ClientSecret": "<client-secret>",
    "SyncOnStartup": true,
    "UpdateExistingContentTypes": true,
    "UpdateExistingDisplayTemplates": true
  }
}
```

### Service Registration

```csharp
builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
```

### Annotation Summary

- **Content types**: `[ContentType("Key", BaseTypes.Element)]`
- **Properties**: `[CMSProperty(DisplayName = "...", Format = PropertyFormats.ShortString)]`
- **Localizable**: `[CultureSpecific]`
- **Choices**: `[CMSPropertyChoice("value", "Display Name")]`
- **Display templates**: Derive from `SaaSDisplayTemplate` with `[DisplayTemplate]`
- **Display settings**: `[DisplayTemplateSetting]` + `[DisplayTemplateChoice]`

See the [Content Types](content-types.md) and [Display Templates](display-templates.md) guides for full documentation.

# SaaS CMS Type Builder

This library lets .NET applications define Optimizely CMS (SaaS) content types as POCOs with attributes. On startup, it scans the application assemblies, compares the discovered definitions with the SaaS CMS API, and creates or updates any missing content types and display templates without deleting anything.

## Configuration

Configure connection settings via the `SaaSCMS` section (or by passing options to `AddSaaSCMSTypeBuilder`):

```json
{
  "SaaSCMS": {
    "ApiBaseUrl": "https://api.cms.optimizely.com/",
    "ApiPathPrefix": "preview3",
    "TokenEndpoint": "/oauth/token",
    "ClientId": "<client-id>",
    "ClientSecret": "<client-secret>",
    "SyncOnStartup": true
  }
}
```

`ApiBaseUrl` and `TokenEndpoint` can point to SaaS or PaaS instances. Use `ContentTypeSources` if you need to restrict which existing types are listed when syncing.

## Startup Sync Behavior

`SaaSCMSTypeSyncHostedService` runs at startup when `SyncOnStartup` is enabled. It:

1. Scans all loaded assemblies for `[ContentType]` and `[DisplayTemplate]` annotations.
2. Fetches existing content types from the SaaS CMS API.
3. Creates missing types and updates existing ones (if `UpdateExistingContentTypes` is enabled).
4. Updates display templates (if `UpdateExistingDisplayTemplates` is enabled).

No deletions are performed.

## Annotation Summary

- **Content types**: `[ContentType("Key", BaseTypes.Component, ...)]`
- **Properties**: `[CMSProperty(DisplayName = "...", Format = "shortString", ...)]`
- **Choices**: `[CMSPropertyChoice("value", "Display Name")]`
- **Display templates**: derive from `SaaSDisplayTemplate` and set `Key`, `ContentType`, `NodeType`, `BaseType`, and `Settings`.

See `GraphTypeBuilderSite` for practical examples.

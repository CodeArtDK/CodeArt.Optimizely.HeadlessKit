# Sample Content Package

This folder contains `meridiandigital.episerverdata` -- an Optimizely CMS content export package with sample pages, elements, and media that match the content types defined in the sample site.

## Importing the Content

1. Log in to your Optimizely CMS SaaS instance
2. Navigate to **Settings** > **Import Data** (or **Tools** > **Import**)
3. Upload `meridiandigital.episerverdata` from this folder
4. Follow the import wizard to complete the process

## Prerequisites

Before importing the content package, make sure:

1. **Content types are synced** -- Run the sample site once with `SyncOnStartup: true` so all content types and display templates are created in your CMS instance
2. **Credentials are configured** -- Your `appsettings.json` (or user secrets) should have valid `SaaSCMS` and `OptimizelyGraph` settings

## What's Included

The content package contains a complete "Meridian Digital" demo site with pages, elements, and media ready to render with the sample site's templates and display settings.

## After Import

Once imported, the content should be available shortly. Run either sample site and navigate to the imported pages to see them rendered with the composition-based templates.

This content package works with both sample sites:
- **Razor Pages** -- `samples/HeadlessKit.Sample.RazorPages/`
- **MVC** -- `samples/HeadlessKit.Sample.Mvc/`

If content appears but isn't rendering correctly:
- Verify that display templates were synced (check startup logs for sync report)
- Verify that `OptimizelyGraph:SingleKey` is correct and the Graph index has been updated
- It may take a few minutes for newly imported content to appear in Optimizely Graph

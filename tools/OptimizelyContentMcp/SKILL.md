# Optimizely CMS Content Management — AI Skill Guide

This document teaches AI agents how to use the Optimizely CMS MCP server to manage content types, create and edit content (including Visual Builder experiences), manage blueprints, changesets, and more.

## Access & Permissions

**If you get 401/403 errors**, the MCP server's OAuth2 credentials may lack the required scopes or the user hasn't granted access. Remind the user to:
1. Verify the `OPTIMIZELY_CLIENT_ID` and `OPTIMIZELY_CLIENT_SECRET` environment variables are set
2. Ensure the OAuth2 application in Optimizely has the required API scopes (content read/write, content types, etc.)
3. Check that the API base URL is correct (`OPTIMIZELY_API_BASE_URL`, defaults to `https://api.cms.optimizely.com/`)

**If you get 404 errors** on content items, double-check the GUID format — the API strips dashes automatically, but the key must be a valid GUID.

---

## Core Concepts

### Content Type Hierarchy

Optimizely CMS SaaS content types have a `baseType` that determines their role:

| baseType | Purpose | Examples |
|---|---|---|
| `_experience` | Full pages with Visual Builder composition | `LandingPage`, `HomePage`, `StandardPage`, `ArticlePage` |
| `_section` | Layout sections inside experiences | `BlankSection` |
| `_component` | Visual elements placed inside sections | `HeadingElement`, `CardElement`, `BannerElement` |
| `_page` | Traditional pages (non-Visual Builder) | Legacy page types |
| `_block` | Reusable content blocks | Shared blocks |
| `_media` | Media files (images, video, documents) | Image, Video types |

### Visual Builder Composition Tree

Experience pages use a nested composition structure. Understanding this tree is essential:

```
experience (root node — the page itself)
  └── section (BlankSection or custom section type)
        └── row
              └── column
                    └── component (the actual element: CardElement, BannerElement, etc.)
```

Every level is required. You cannot place a component directly in a section — it must be nested inside `section → row → column → component`.

### Content Identification

- Every content item has a **key** (GUID, e.g. `c6f983720c9249b6bfeaae688d14abd0`)
- Content is placed under a **container** (parent GUID)
- Each item can have multiple **versions** and **locales**
- The `routeSegment` determines the URL slug

---

## Tool Reference

### Content Types

#### Discover what types exist
```
list_content_types(pageSize=100)
```
Returns all content types. Use `sources="code"` or `sources="ui"` to filter. The response can be very large — extract just the `key`, `baseType`, and `properties` you need.

#### Inspect a specific type before creating content
```
get_content_type(key="LandingPage")
```
**Always do this before creating content.** It reveals:
- `properties` — the exact property names, types, and formats you must use
- `mayContainTypes` — which element types this experience allows
- `compositionBehaviors` — e.g. `elementEnabled`, `sectionEnabled`

#### Create a new content type
```
create_content_type(body={
  "key": "ProductCard",
  "displayName": "Product Card",
  "baseType": "_component",
  "compositionBehaviors": ["elementEnabled"],
  "properties": {
    "Name": { "type": "string", "format": "shortString", "displayName": "Name", "localized": true },
    "Price": { "type": "string", "format": "shortString", "displayName": "Price" },
    "Description": { "type": "richText", "displayName": "Description", "localized": true }
  }
})
```

#### Update a content type (merge-patch — only include changed fields)
```
update_content_type(key="ProductCard", body={
  "properties": {
    "Rating": { "type": "string", "format": "shortString", "displayName": "Rating" }
  }
})
```

### Property Types & Formats

Common property type/format combinations:

| type | format | Use for |
|---|---|---|
| `string` | `shortString` | Single-line text (titles, names, labels) |
| `string` | *(none)* | Multi-line plain text (descriptions) |
| `richText` | *(none)* | HTML rich text (body content) |
| `boolean` | *(none)* | True/false toggles |
| `number` | *(none)* | Numeric values |
| `url` | *(none)* | Links and URLs |
| `contentReference` | *(none)* | Reference to another content item (images, pages) |
| `contentArea` | *(none)* | Drag-and-drop area for multiple content items |

Use `list_property_formats()` to see all available formats.

---

## Creating Content

### Creating a Simple Content Item

```
create_content(body={
  "contentType": "StandardPage",
  "container": "<parent-guid>",
  "locale": "en",
  "displayName": "About Us",
  "status": "draft",
  "routeSegment": "about-us",
  "properties": {
    "Title": "About Us",
    "Body": "<p>Welcome to our company.</p>"
  }
})
```

### Creating a Visual Builder Experience (Full Page with Sections & Elements)

This is the most complex operation. Here's the complete pattern:

```
create_content(body={
  "contentType": "LandingPage",
  "container": "<parent-guid>",
  "locale": "en",
  "displayName": "My Landing Page",
  "status": "draft",
  "routeSegment": "my-landing-page",
  "composition": {
    "displayName": "My Landing Page",
    "nodeType": "experience",
    "layoutType": "outline",
    "nodes": [
      {
        "displayName": "Hero Section",
        "nodeType": "section",
        "layoutType": "grid",
        "component": {
          "contentType": "BlankSection",
          "properties": {}
        },
        "nodes": [
          {
            "displayName": "Row",
            "nodeType": "row",
            "nodes": [
              {
                "displayName": "Column",
                "nodeType": "column",
                "nodes": [
                  {
                    "displayName": "Welcome Banner",
                    "nodeType": "component",
                    "component": {
                      "contentType": "BannerElement",
                      "properties": {
                        "Heading": "Welcome to Our Site",
                        "Body": "<p>This is the hero section.</p>",
                        "LinkText": "Learn More"
                      }
                    }
                  }
                ]
              }
            ]
          }
        ]
      }
    ]
  },
  "properties": {
    "SiteName": "My Site"
  }
})
```

#### Key rules for composition:

1. **Root node** must be `"nodeType": "experience"` with `"layoutType": "outline"`
2. **Sections** must have `"nodeType": "section"`, `"layoutType": "grid"`, and a `component` specifying the section type (usually `BlankSection`)
3. **Rows** have `"nodeType": "row"` — no component needed
4. **Columns** have `"nodeType": "column"` — no component needed
5. **Components** have `"nodeType": "component"` and a `component` object with `contentType` and `properties`
6. You do **not** need to provide `id` values — the server generates them
7. Every component's `contentType` must be listed in the experience type's `mayContainTypes` array

#### Multi-column layouts

For side-by-side content (e.g., 3 product cards), create multiple columns within a single row:

```json
{
  "displayName": "Products Row",
  "nodeType": "row",
  "nodes": [
    {
      "displayName": "Col 1",
      "nodeType": "column",
      "nodes": [{ "nodeType": "component", "component": { "contentType": "CardElement", "properties": { "Title": "Product A" } } }]
    },
    {
      "displayName": "Col 2",
      "nodeType": "column",
      "nodes": [{ "nodeType": "component", "component": { "contentType": "CardElement", "properties": { "Title": "Product B" } } }]
    },
    {
      "displayName": "Col 3",
      "nodeType": "column",
      "nodes": [{ "nodeType": "component", "component": { "contentType": "CardElement", "properties": { "Title": "Product C" } } }]
    }
  ]
}
```

#### Stacking elements vertically

Place multiple components in the same column:

```json
{
  "displayName": "Column",
  "nodeType": "column",
  "nodes": [
    { "nodeType": "component", "component": { "contentType": "HeadingElement", "properties": { "Text": "Title", "Level": "h2" } } },
    { "nodeType": "component", "component": { "contentType": "HtmlBlockElement", "properties": { "Body": "<p>Paragraph text.</p>" } } }
  ]
}
```

---

## Navigating Content

### Finding the root / parent content

There is no universal root GUID. To find where to place content:

1. **If you know a page's GUID**, list its children: `list_content_children(key="<guid>")`
2. **If you have a GUID from a previous creation**, use that as the `container` for child pages
3. **Ask the user** for the container GUID if you cannot discover it

### Getting content
```
get_content(key="<guid>")              # Get with all properties
get_content(key="<guid>", locale="sv") # Get specific locale
```

### Listing children
```
list_content_children(key="<parent-guid>", pageSize=50)
```

### Getting the URL path
```
get_content_path(key="<guid>")
```

---

## Updating Content

### Update properties (merge-patch — only send what changed)
```
update_content(key="<guid>", body={
  "properties": {
    "SiteName": "New Site Name"
  }
})
```

### Create a new draft version
```
create_content_version(key="<guid>", body={
  "status": "draft",
  "properties": {
    "Title": "Updated Title"
  }
})
```

### Update a specific version
```
update_content_version(key="<guid>", version="<version-id>", body={
  "properties": { "Title": "Fixed Title" }
})
```

---

## Version Management

```
list_content_versions(key="<guid>")                    # List all versions
get_content_version(key="<guid>", version="<ver>")     # Get specific version
delete_content_version(key="<guid>", version="<ver>")  # Delete a version
delete_content_locale(key="<guid>", locale="sv")       # Delete all versions for a locale
```

---

## Blueprints (Content Templates)

Blueprints are pre-filled content items that editors use as starting points.

```
# List all blueprints
list_blueprints()

# Create a blueprint (same structure as content, but stored as a template)
create_blueprint(body={
  "key": "standard-article",
  "contentType": "ArticlePage",
  "displayName": "Standard Article Template",
  "properties": {
    "Title": "Article Title Here",
    "Body": "<p>Start writing your article...</p>"
  }
})

# Update / delete
update_blueprint(key="standard-article", body={ "displayName": "Updated Name" })
delete_blueprint(key="standard-article")
```

---

## Changesets (Batch Publishing)

Changesets group related content changes for publishing together.

```
# Create a changeset
create_changeset(body={ "name": "Spring Campaign", "description": "All spring content updates" })

# Add content to it
add_changeset_item(id="<changeset-id>", body={ "contentKey": "<guid>", "version": "<ver>" })

# List items in a changeset
list_changeset_items(id="<changeset-id>")

# Clean up
delete_changeset_item(id="<changeset-id>", itemId="<item-id>")
delete_changeset(id="<changeset-id>")
```

---

## Display Templates

```
list_display_templates()
get_display_template(id="<id>")
create_display_template(body={
  "key": "CardWide",
  "contentType": "CardElement",
  "settings": []
})
```

---

## Property Groups

Property groups organize properties into tabs in the CMS editor UI.

```
list_property_groups()
create_property_group(body={ "key": "SEO", "displayName": "SEO Settings" })
```

---

## Import / Export

```
# Export all content as a package
export_package()  # Returns a package key

# Check export status
get_package_status(key="<package-key>")

# Import a package
import_package(body={ ... package data ... })
```

---

## Recommended Workflow for Creating a Full Page

1. **Discover available types**: `list_content_types(pageSize=100)` — note the `_experience` types for pages and `_component` types for elements
2. **Inspect the target page type**: `get_content_type(key="LandingPage")` — check `properties` and `mayContainTypes`
3. **Inspect element types you plan to use**: `get_content_type(key="CardElement")` etc. — get exact property names and types
4. **Identify the parent container**: Ask the user or use `list_content_children` to navigate the tree
5. **Build the composition tree**: Follow the `experience → section → row → column → component` structure strictly
6. **Create the content**: `create_content(body={...})` with the full composition and properties
7. **Verify**: `get_content(key="<returned-guid>")` to confirm it was created correctly

## Common Mistakes to Avoid

- **Using content types that don't exist** — always check `list_content_types` first
- **Wrong property names** — always check `get_content_type` for exact property names (e.g., `Text` not `Heading` on `HeadingElement`)
- **Skipping the row/column nesting** — components MUST be inside `section → row → column`, never directly in a section
- **Providing IDs in composition nodes** — let the server generate them; providing invalid GUIDs causes errors
- **Forgetting `"component"` on sections** — sections need `"component": { "contentType": "BlankSection", "properties": {} }`
- **Using element types not in `mayContainTypes`** — the experience type defines which elements are allowed
- **Rich text without HTML tags** — `richText` properties expect HTML strings like `"<p>Text here</p>"`
- **Mixing up `container` and `parentLink`** — use `container` with the parent GUID when creating content

## Troubleshooting

| Error | Cause | Fix |
|---|---|---|
| 401 Unauthorized | OAuth2 token expired or invalid credentials | Check `OPTIMIZELY_CLIENT_ID` and `OPTIMIZELY_CLIENT_SECRET` env vars. Remind the user to grant API access rights. |
| 403 Forbidden | Insufficient permissions | The OAuth2 app needs content management scopes. Ask the user to update permissions in Optimizely. |
| 404 Not Found | Invalid GUID or content doesn't exist | Verify the GUID. Use `list_content_children` to find valid keys. |
| 400 Bad Request | Malformed body or invalid property names | Check `get_content_type` for correct property names and types. |
| 409 Conflict | Content with same route segment exists | Use a different `routeSegment` value. |

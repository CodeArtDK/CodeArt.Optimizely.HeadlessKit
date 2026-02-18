# AI Instructions: Optimizely SaaS CMS API Direct Access

This document helps AI coding assistants (Claude Code, GitHub Copilot, etc.) interact directly with the Optimizely SaaS CMS APIs for creating content, managing types, and querying content via GraphQL.

## Authentication

Optimizely SaaS CMS uses OAuth2 client credentials flow.

### Getting an Access Token

```bash
curl -X POST "https://api.cms.optimizely.com/oauth/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=YOUR_CLIENT_ID&client_secret=YOUR_CLIENT_SECRET"
```

Response:
```json
{
  "access_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

Use the token in subsequent requests:
```
Authorization: Bearer eyJ...
```

### Credential Sources

If the project uses HeadlessKit, credentials are in `appsettings.json`:
- `SaaSCMS:ClientId` and `SaaSCMS:ClientSecret` for the REST API
- `OptimizelyGraph:SingleKey` for Graph queries

Or in .NET user secrets:
```bash
dotnet user-secrets list --project <path-to-csproj>
```

## REST API: Content Types

Base URL: `https://api.cms.optimizely.com/preview3`

### List Content Types

```bash
curl -H "Authorization: Bearer TOKEN" \
  "https://api.cms.optimizely.com/preview3/contenttypes"
```

### Get a Content Type

```bash
curl -H "Authorization: Bearer TOKEN" \
  "https://api.cms.optimizely.com/preview3/contenttypes/StandardPage"
```

### Create a Content Type

```bash
curl -X POST "https://api.cms.optimizely.com/preview3/contenttypes" \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "key": "HeroElement",
    "displayName": "Hero Element",
    "baseType": "element",
    "sortOrder": 100,
    "features": ["localization", "versioning", "publishPeriod"],
    "usages": ["property", "instance"],
    "properties": {
      "title": {
        "type": "string",
        "format": "shortString",
        "displayName": "Title",
        "localized": true,
        "sortOrder": 10
      },
      "subtitle": {
        "type": "string",
        "format": "shortString",
        "displayName": "Subtitle",
        "sortOrder": 20
      },
      "backgroundImage": {
        "type": "contentReference",
        "displayName": "Background Image",
        "sortOrder": 30
      },
      "buttonLink": {
        "type": "url",
        "displayName": "Button Link",
        "sortOrder": 40
      }
    }
  }'
```

### Update a Content Type (Merge Patch)

```bash
curl -X PATCH "https://api.cms.optimizely.com/preview3/contenttypes/HeroElement" \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/merge-patch+json" \
  -d '{
    "displayName": "Hero Banner Element",
    "properties": {
      "buttonText": {
        "type": "string",
        "format": "shortString",
        "displayName": "Button Text",
        "sortOrder": 50
      }
    }
  }'
```

To ignore data loss warnings (e.g., when changing property types):
```
cms-ignore-data-loss-warnings: true
```

### Content Type JSON Schema

```json
{
  "key": "string (required, unique identifier)",
  "displayName": "string",
  "description": "string",
  "baseType": "page | block | media | image | video | folder | experience | section | element",
  "sortOrder": 0,
  "features": ["localization", "versioning", "publishPeriod", "routing", "binary"],
  "usages": ["property", "instance"],
  "mayContainTypes": ["ElementKey1", "ElementKey2"],
  "properties": {
    "propertyName": {
      "type": "string | integer | boolean | dateTime | float | array | contentReference | richText | url",
      "format": "shortString | selectOne | listOfString | imageUrl | documentUrl | html",
      "displayName": "string",
      "description": "string",
      "group": "string",
      "sortOrder": 0,
      "localized": false,
      "required": false,
      "indexingType": "string",
      "allowedTypes": ["TypeKey"],
      "restrictedTypes": ["TypeKey"],
      "enum": {
        "value1": { "displayName": "Label 1", "sortOrder": 1 },
        "value2": { "displayName": "Label 2", "sortOrder": 2 }
      },
      "validationRegex": "pattern",
      "maxLength": 200,
      "minLength": 0,
      "maximum": 100,
      "minimum": 0,
      "items": {
        "type": "string",
        "format": "shortString"
      }
    }
  }
}
```

## REST API: Display Templates

### List Display Templates

```bash
curl -H "Authorization: Bearer TOKEN" \
  "https://api.cms.optimizely.com/preview3/displaytemplates"
```

### Create a Display Template

```bash
curl -X POST "https://api.cms.optimizely.com/preview3/displaytemplates" \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "key": "SectionDefault",
    "displayName": "Section",
    "baseType": "section",
    "isDefault": true,
    "settings": {
      "colorScheme": {
        "displayName": "Color Scheme",
        "editor": "choice",
        "sortOrder": 10,
        "choices": {
          "default": { "displayName": "Default", "sortOrder": 1 },
          "dark": { "displayName": "Dark", "sortOrder": 2 },
          "light": { "displayName": "Light", "sortOrder": 3 }
        }
      }
    }
  }'
```

### Update a Display Template

```bash
curl -X PATCH "https://api.cms.optimizely.com/preview3/displaytemplates/SectionDefault" \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/merge-patch+json" \
  -d '{
    "settings": {
      "padding": {
        "displayName": "Padding",
        "editor": "choice",
        "sortOrder": 20,
        "choices": {
          "default": { "displayName": "Default", "sortOrder": 1 },
          "compact": { "displayName": "Compact", "sortOrder": 2 },
          "spacious": { "displayName": "Spacious", "sortOrder": 3 }
        }
      }
    }
  }'
```

## GraphQL API: Optimizely Graph

### Endpoint

```
https://cg.optimizely.com/content/v2?auth=YOUR_SINGLE_KEY
```

For preview/draft content, use Bearer token auth instead:
```bash
curl -X POST "https://cg.optimizely.com/content/v2" \
  -H "Authorization: Bearer ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"query": "..."}'
```

### Query Published Content by Path

```graphql
query GetByPath($path: String!, $locale: [Locales]) {
  _Experience(
    where: { _metadata: { url: { default: { eq: $path } } } }
    locale: $locale
  ) {
    items {
      _metadata {
        key
        locale
        types
        displayName
        version
        url { default hierarchical }
        status
        published
      }
      ... on StandardPage {
        MetaTitle
        MetaDescription
      }
      ... on ArticlePage {
        Title
        Excerpt
        FeaturedImage { url { default } }
        Body { html }
        PublishedDate
        Tags
      }
    }
  }
}
```

Variables:
```json
{
  "path": "/en/articles/my-article",
  "locale": ["en"]
}
```

### Query Content by Key

```graphql
query GetByKey($key: String!, $locale: [Locales]) {
  _Experience(
    where: { _metadata: { key: { eq: $key } } }
    locale: $locale
  ) {
    items {
      _metadata { key types displayName url { default } }
      ... on StandardPage { MetaTitle }
    }
  }
}
```

### List Content with Filters

```graphql
query ListArticles($status: String, $limit: Int) {
  ArticlePage(
    where: { _metadata: { status: { eq: $status } } }
    orderBy: { PublishedDate: DESC }
    limit: $limit
  ) {
    items {
      _metadata { key url { default } }
      Title
      Excerpt
      PublishedDate
      Tags
    }
    total
    cursor
  }
}
```

### Full-Text Search

```graphql
query Search($query: String!, $limit: Int) {
  ArticlePage(
    where: { _fulltext: { match: $query } }
    limit: $limit
  ) {
    items {
      _metadata { key url { default } }
      Title
      Excerpt
    }
    total
    facets {
      Tags { name count }
    }
  }
}
```

### Query with Composition (Visual Builder)

```graphql
query GetExperience($key: String!) {
  _Experience(where: { _metadata: { key: { eq: $key } } }) {
    items {
      _metadata { key types url { default } }
      ... on StandardPage {
        MetaTitle
        _metadata {
          composition {
            nodeType
            displayTemplateKey
            displaySettings { key value }
            nodes {
              ... on CompositionStructureNode {
                type
                nodeType
                key
                displayTemplateKey
                displaySettings { key value }
                nodes {
                  ... on CompositionComponentNode {
                    type
                    component {
                      _metadata { types }
                      ... on HeroElement {
                        Title
                        Subtitle
                        BackgroundImage { url { default } }
                      }
                      ... on BannerElement {
                        Heading
                        Body { html }
                        Image { url { default } }
                      }
                    }
                    displayTemplateKey
                    displaySettings { key value }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}
```

## Common Patterns

### Creating a Complete Content Type Setup

1. Authenticate to get a token
2. Create content types (elements first, then pages that reference them via `mayContainTypes`)
3. Create display templates for sections and elements
4. Query content via Graph to verify

### Checking Sync Status

If HeadlessKit is running, types sync on startup. To manually check:
- List remote types: `GET /preview3/contenttypes`
- Compare with local `[ContentType]`-annotated classes
- Differences are logged at startup

### API Rate Limits

The CMS REST API may have rate limits. When creating many types programmatically:
- Add small delays between requests
- Create types in dependency order (referenced types first)
- Use merge-patch (PATCH) for updates to minimize payload

## Useful GraphQL Introspection

Get available types:
```graphql
{
  __schema {
    types {
      name
      kind
      fields { name type { name } }
    }
  }
}
```

Get fields for a specific type:
```graphql
{
  __type(name: "ArticlePage") {
    fields { name type { name kind } }
  }
}
```

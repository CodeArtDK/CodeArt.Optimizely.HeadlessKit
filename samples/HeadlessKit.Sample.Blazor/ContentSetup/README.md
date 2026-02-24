# Blazor Portal - CMS Content Setup

This guide explains how to create the portal content tree in Optimizely CMS for the Blazor sample.

## Prerequisites

1. Content types must exist in the CMS. Run the seed tool first:
   ```bash
   dotnet run --project tools/SeedPortalContent
   ```
   Or just run the Blazor app once (it syncs types on startup).

2. You need CMS editor/admin access to create content items and set up compositions.

## Content Specification

`content-items.json` is the canonical specification of all content that must be created.
It is a JSON array where each entry describes one content item:

| Field | Description |
|-------|-------------|
| `contentType` | The CMS content type key |
| `displayName` | The item's display name in the CMS |
| `locale` | Language locale (`en`) |
| `container` | Key of the parent container (see placeholders below) |
| `routeSegment` | URL segment (empty string = root `/`) |
| `properties` | Property values for the content type |
| `composition.sections` | Visual Builder sections with element instances |

### Placeholder values to replace

| Placeholder | Replace with |
|-------------|--------------|
| `ROOT_CONTAINER_KEY` | The key of your root site container in the CMS |
| `PORTAL_DASHBOARD_CONTENT_KEY` | The key of the **CloudPulse Portal** dashboard after creating it |

## Content Tree Structure

```
Root Container
├── CloudPulse Welcome (PortalDashboard)   ← routeSegment: "" (CMS URL = /)
└── CloudPulse Portal (PortalDashboard)    ← routeSegment: "portal" (CMS URL = /portal)
    ├── Account (PortalPage)               ← routeSegment: "account" (CMS URL = /portal/account)
    └── Billing (PortalPage)               ← routeSegment: "billing" (CMS URL = /portal/billing)
```

## Visual Builder Setup

Use the content specification in `content-items.json` as the authoritative reference for property values and composition layouts. The steps below describe the creation workflow.

### Step 1: Create the Start Page (public welcome)

1. Open the CMS Visual Builder
2. Create a new **PortalDashboard** experience directly under the root container
3. Use the properties from `content-items.json` → first item (`displayName: "CloudPulse Welcome"`)
4. Set **Route Segment** to empty — this makes its CMS URL `/`
5. Optionally add composition elements (see `composition.sections` in the JSON)

> This page is shown at `/` before login with no sidebar. If no CMS content exists at `/`, a built-in fallback welcome page is shown.

### Step 2: Create the Dashboard page

1. Create a new **PortalDashboard** experience directly under the root container (sibling to the start page)
2. Use the properties from `content-items.json` → second item (`displayName: "CloudPulse Portal"`)
3. Set **Route Segment** to `portal`
4. In the Visual Builder composition editor, add the sections and elements from `composition.sections` in the JSON

### Step 3: Create the Account page

1. Create a new **PortalPage** experience under the **CloudPulse Portal** dashboard
2. Use the properties from `content-items.json` → third item (`displayName: "Account"`)
3. Set **Route Segment** to `account`
4. Add composition sections and elements as specified in the JSON

### Step 4: Create the Billing page

1. Create a new **PortalPage** experience under the **CloudPulse Portal** dashboard
2. Use the properties from `content-items.json` → fourth item (`displayName: "Billing"`)
3. Set **Route Segment** to `billing`
4. Add composition sections and elements as specified in the JSON

### Step 5: Publish all pages

Publish all four pages. Content may take a few minutes to appear in Optimizely Graph.

## Data Keys Reference

The portal elements use `DataKey` properties to look up user-specific data from `Data/users.json`:

### UsageStatElement DataKeys
| DataKey | Source | Demo Value |
|---|---|---|
| `ApiCalls` | `user.UsageData["ApiCalls"]` | 8420 |
| `Storage` | `user.UsageData["Storage"]` | 67 |
| `Bandwidth` | `user.UsageData["Bandwidth"]` | 234 |
| `Projects` | `user.UsageData["Projects"]` | 12 |

### AccountDetailElement DataKeys
| DataKey | Source | Demo Value |
|---|---|---|
| `Email` | `user.Email` | alex.morgan@example.com |
| `Plan` | `user.Plan` | Professional |
| `DisplayName` | `user.DisplayName` | Alex Morgan |
| `MemberSince` | `user.MemberSince` (formatted as "MMMM yyyy") | March 2024 |
| `Username` | `user.Username` | demo |

## Demo Login

- **Username**: `demo`
- **Password**: `demo123`


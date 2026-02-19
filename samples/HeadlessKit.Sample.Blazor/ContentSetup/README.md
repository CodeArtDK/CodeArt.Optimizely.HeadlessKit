# Blazor Portal - CMS Content Setup

This guide explains how to create the portal content tree in Optimizely CMS for the Blazor sample.

## Prerequisites

1. Content types must exist in the CMS. Run the seed tool first:
   ```bash
   dotnet run --project tools/SeedPortalContent
   ```
   Or just run the Blazor app once (it syncs types on startup).

2. You need CMS editor/admin access to create content items and set up compositions.

## Content Tree Structure

```
Root Container
├── Meridian Digital (LandingPage)         ← existing demo site
│   ├── Services, About Us, etc.
├── CloudPulse Welcome (PortalDashboard)   ← NEW, routeSegment: "" (start page, CMS URL = /)
└── CloudPulse Portal (PortalDashboard)    ← NEW, routeSegment: "portal"
    ├── Account (PortalPage)               ← routeSegment: "account"
    └── Billing (PortalPage)               ← routeSegment: "billing"
```

## API Content Creation

If you have content management API permissions, use the JSON payloads in `content-items.json`.

```bash
# Get a token
TOKEN=$(curl -s -X POST "https://api.cms.optimizely.com/oauth/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=YOUR_CLIENT_ID&client_secret=YOUR_SECRET" \
  | jq -r '.access_token')

# Create dashboard (replace CONTAINER with your root container key)
curl -X POST "https://api.cms.optimizely.com/preview3/experimental/content" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d @content-items.json
```

> **Note:** The `preview3/experimental/content` endpoint requires content-level write
> permissions on the target container. The API client may only have type-management
> permissions. If you get 403, create content through the CMS UI instead.

## Visual Builder Setup (Recommended)

### Step 0: Create the Start Page (public welcome)

1. Open the CMS Visual Builder
2. Create a new **PortalDashboard** experience under the root (sibling to "Meridian Digital")
3. Set:
   - **Display Name**: `CloudPulse Welcome`
   - **Route Segment**: *(leave empty)* — this makes its CMS URL `/`
   - **Portal Name**: `CloudPulse`
   - **Support Email**: `support@cloudpulse.example.com`

4. Optionally add a composition with public-facing elements (e.g. InfoCards, Announcements).
   User-specific elements (UsageStat, AccountDetail, BillingHistory) will render without data
   since this is a public page with no logged-in user.

> This page is shown at `/` before login. The Blazor app renders it with `[AllowAnonymous]`
> and no sidebar. If no CMS content exists at `/`, a built-in fallback welcome page is shown.

### Step 1: Create the Dashboard page

1. Open the CMS Visual Builder
2. Create a new **PortalDashboard** experience under the root (sibling to "Meridian Digital")
3. Set:
   - **Display Name**: `CloudPulse Portal`
   - **Route Segment**: `portal`
   - **Portal Name**: `CloudPulse`
   - **Support Email**: `support@cloudpulse.example.com`

4. Add a composition with these elements:

   **Section 1 - Welcome Banner (full width):**
   | Element Type | Property | Value |
   |---|---|---|
   | WelcomeBannerElement | Greeting Template | `Welcome back, {name}!` |
   | | Subtitle | `Here's your portal overview` |
   | | Icon Class | `☁` |

   **Section 2 - Usage Stats (4 columns):**
   | Element Type | Label | Unit | Icon Class | Max Value | Data Key |
   |---|---|---|---|---|---|
   | UsageStatElement | API Calls | / mo | ⚡ | 50000 | `ApiCalls` |
   | UsageStatElement | Storage | GB | ☁ | 100 | `Storage` |
   | UsageStatElement | Bandwidth | GB | ↔ | 500 | `Bandwidth` |
   | UsageStatElement | Projects | active | ☰ | 25 | `Projects` |

   **Section 3 - Info Cards (3 columns):**
   | Element Type | Title | Description | Icon Class | Link Text |
   |---|---|---|---|---|
   | InfoCardElement | Documentation | Browse our API docs and guides | 📄 | View Docs |
   | InfoCardElement | Support | Contact our team for help | 💬 | Get Help |
   | InfoCardElement | Status | Check system health and uptime | ✅ | View Status |

   **Section 4 - Announcement (full width):**
   | Element Type | Title | Body | Severity | Icon Class |
   |---|---|---|---|---|
   | AnnouncementElement | System Update | Scheduled maintenance on March 1st, 2026 from 02:00-04:00 UTC. | info | ℹ |

### Step 2: Create the Account page

1. Create a new **PortalPage** experience under the CloudPulse Portal dashboard
2. Set:
   - **Display Name**: `Account`
   - **Route Segment**: `account`
   - **Page Title**: `Account Settings`
   - **Page Description**: `View and manage your account details`

3. Add a composition with these elements:

   **Section 1 - Account Details (2x2 grid):**
   | Element Type | Label | Icon Class | Display Format | Data Key |
   |---|---|---|---|---|
   | AccountDetailElement | Email | ✉ | | `Email` |
   | AccountDetailElement | Plan | ★ | | `Plan` |
   | AccountDetailElement | Display Name | ☺ | | `DisplayName` |
   | AccountDetailElement | Member Since | ✎ | | `MemberSince` |

   **Section 2 - Announcement (full width):**
   | Element Type | Title | Body | Severity | Icon Class |
   |---|---|---|---|---|
   | AnnouncementElement | Two-Factor Auth | Enable 2FA for enhanced account security. | warning | ⚠ |

### Step 3: Create the Billing page

1. Create a new **PortalPage** experience under the CloudPulse Portal dashboard
2. Set:
   - **Display Name**: `Billing`
   - **Route Segment**: `billing`
   - **Page Title**: `Billing & Invoices`
   - **Page Description**: `View your billing history and manage payment methods`

3. Add a composition with these elements:

   **Section 1 - Billing History (full width):**
   | Element Type | Property | Value |
   |---|---|---|
   | BillingHistoryElement | Title | `Billing History` |
   | | Date Header | `Date` |
   | | Description Header | `Description` |
   | | Amount Header | `Amount` |
   | | Status Header | `Status` |
   | | Empty Message | `No billing history available.` |

   **Section 2 - Info Card (full width):**
   | Element Type | Title | Description | Icon Class | Link Text |
   |---|---|---|---|---|
   | InfoCardElement | Payment Methods | Manage your credit cards and payment options | 💳 | Manage Payments |

### Step 4: Publish all pages

Publish all three pages (Dashboard, Account, Billing). Content may take a few minutes to appear in Optimizely Graph.

## Data Keys Reference

The portal elements use `DataKey` properties to look up user-specific data from `users.json`:

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
| `MemberSince` | `user.MemberSince` (formatted) | March 2024 |
| `Username` | `user.Username` | demo |

## Demo Login

- **Username**: `demo`
- **Password**: `demo123`

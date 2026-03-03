# Optimizely CMS MCP Server

A [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) server that gives AI assistants like Claude full access to manage content in Optimizely CMS (SaaS). Create pages, manage content types, build Visual Builder experiences, handle versions, and more — all through natural language.

## What Can It Do?

- **Content Types** — List, inspect, create, update, and delete content type definitions
- **Content Management** — Create, read, update, and delete content items including full Visual Builder experiences with sections, rows, columns, and elements
- **Version Management** — List, create, and manage content versions and locales
- **Blueprints** — Create and manage content templates for editors
- **Changesets** — Group content changes for batch publishing
- **Display Templates** — Manage editor display settings
- **Import/Export** — Package content for migration

## Prerequisites

- An Optimizely CMS (SaaS) instance
- An OAuth2 application configured in Optimizely with content management API scopes
- Your `Client ID` and `Client Secret` from the Optimizely dashboard

## Setup

### 1. Download

Download the latest release for your platform from the [GitHub Releases](https://github.com/CodeArtDK/CodeArt.Optimizely.HeadlessKit/releases) page and extract the zip.

### 2. Configure with Claude Desktop

Add the following to your Claude Desktop config file:

- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "optimizely-cms": {
      "command": "/path/to/OptimizelyContentMcp",
      "env": {
        "OPTIMIZELY_CLIENT_ID": "<your-client-id>",
        "OPTIMIZELY_CLIENT_SECRET": "<your-client-secret>"
      }
    }
  }
}
```

On **Windows**, use the `.exe` path with double backslashes:

```json
{
  "mcpServers": {
    "optimizely-cms": {
      "command": "C:\\tools\\OptimizelyContentMcp.exe",
      "env": {
        "OPTIMIZELY_CLIENT_ID": "<your-client-id>",
        "OPTIMIZELY_CLIENT_SECRET": "<your-client-secret>"
      }
    }
  }
}
```

### 3. Configure with Claude Code (CLI)

Add the MCP server to your project or global settings:

```bash
# Add to current project
claude mcp add optimizely-cms -- /path/to/OptimizelyContentMcp

# Or add globally
claude mcp add --global optimizely-cms -- /path/to/OptimizelyContentMcp
```

Then set the environment variables before running Claude Code:

```bash
export OPTIMIZELY_CLIENT_ID="<your-client-id>"
export OPTIMIZELY_CLIENT_SECRET="<your-client-secret>"
```

Or on Windows (PowerShell):

```powershell
$env:OPTIMIZELY_CLIENT_ID = "<your-client-id>"
$env:OPTIMIZELY_CLIENT_SECRET = "<your-client-secret>"
```

### 4. Configure with VS Code (GitHub Copilot / Continue)

Add to your `.vscode/mcp.json`:

```json
{
  "servers": {
    "optimizely-cms": {
      "command": "/path/to/OptimizelyContentMcp",
      "env": {
        "OPTIMIZELY_CLIENT_ID": "<your-client-id>",
        "OPTIMIZELY_CLIENT_SECRET": "<your-client-secret>"
      }
    }
  }
}
```

## Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `OPTIMIZELY_CLIENT_ID` | Yes | — | OAuth2 Client ID from Optimizely |
| `OPTIMIZELY_CLIENT_SECRET` | Yes | — | OAuth2 Client Secret from Optimizely |
| `OPTIMIZELY_API_BASE_URL` | No | `https://api.cms.optimizely.com/` | CMS API base URL |
| `OPTIMIZELY_API_VERSION` | No | `preview3` | API version path segment |

## Available Tools

The MCP server exposes the following tool groups:

| Category | Tools | Description |
|----------|-------|-------------|
| Content Types | `list_content_types`, `get_content_type`, `create_content_type`, `update_content_type`, `delete_content_type` | Manage CMS content type definitions |
| Content | `get_content`, `create_content`, `update_content`, `delete_content`, `copy_content`, `undelete_content` | CRUD operations on content items |
| Content Navigation | `list_content_children`, `list_content_assets`, `get_content_path` | Navigate the content tree |
| Versions | `list_content_versions`, `get_content_version`, `create_content_version`, `update_content_version`, `delete_content_version`, `delete_content_locale`, `query_content_versions` | Manage content versions |
| Blueprints | `list_blueprints`, `get_blueprint`, `create_blueprint`, `update_blueprint`, `delete_blueprint` | Content templates for editors |
| Changesets | `list_changesets`, `get_changeset`, `create_changeset`, `update_changeset`, `delete_changeset`, `list_changeset_items`, `get_changeset_item`, `add_changeset_item`, `update_changeset_item`, `delete_changeset_item` | Batch publishing workflows |
| Display Templates | `list_display_templates`, `get_display_template`, `create_display_template`, `update_display_template`, `delete_display_template` | Editor display settings |
| Property Formats | `list_property_formats`, `get_property_format` | Available data types |
| Property Groups | `list_property_groups`, `get_property_group`, `create_property_group`, `update_property_group`, `delete_property_group` | Editor UI organization |
| Packages | `export_package`, `import_package`, `get_package_status` | Content import/export |

## AI Skill Guide

The included `SKILL.md` file is a comprehensive guide that teaches AI assistants how to use these tools effectively. It covers:

- Content type hierarchy and Visual Builder composition structure
- Step-by-step workflows for creating full pages with elements
- Property types and formats reference
- Common mistakes to avoid
- Troubleshooting guide

You can include `SKILL.md` as context in your AI prompts to get better results when working with Optimizely CMS content.

## Troubleshooting

### 401 Unauthorized
Your OAuth2 credentials are invalid or expired. Verify `OPTIMIZELY_CLIENT_ID` and `OPTIMIZELY_CLIENT_SECRET` are set correctly.

### 403 Forbidden
The OAuth2 application lacks the required API scopes. Go to the Optimizely dashboard and ensure your application has content management permissions.

### Connection refused / server not starting
- Ensure the executable has execute permissions on macOS/Linux: `chmod +x OptimizelyContentMcp`
- Check that no other process is using the same stdio transport
- Verify the path in your MCP configuration is correct

### Tools not appearing
Restart your AI client (Claude Desktop, VS Code, etc.) after updating the MCP configuration. The server registers tools on startup.

## Building from Source

Requires .NET 10 SDK.

```bash
# Build
dotnet build tools/OptimizelyContentMcp/OptimizelyContentMcp.csproj

# Run directly
dotnet run --project tools/OptimizelyContentMcp/OptimizelyContentMcp.csproj

# Publish self-contained executable
dotnet publish tools/OptimizelyContentMcp/OptimizelyContentMcp.csproj \
  -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## License

MIT — see [LICENSE](../../LICENSE) for details.

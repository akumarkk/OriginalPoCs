# Dynatrace Token Reference & Usage Guide

This guide details all token types used in Dynatrace, their prefix formats, primary use cases, and how to generate and use them in automation scripts and API requests.

---

## Token Structure Overview

Modern Dynatrace tokens follow a standard three-part format:

$$
\text{Prefix} . \text{PublicIdentifier} . \text{SecretKey}
$$

* **Prefix:** Identifies the specific token type and engine (e.g., `dt0c01`, `dt0s16`, `dt0a01`).
* **Public Identifier:** A unique string used for identification, auditing, and lookup in Dynatrace logs without exposing the secret key.
* **Secret Key:** The cryptographic credential used for request authentication.

---

## Complete Token Matrix

| Token Type | Prefix | Primary Purpose & Scope | How to Generate / Obtain | Required Permissions |
| :--- | :--- | :--- | :--- | :--- |
| **Platform Access Token** | `dt0s16` | Programmatic access to Dynatrace Platform (Grail, OpenPipeline, Automation Engine, Notebooks). | Web UI: **Profile/Account** $\rightarrow$ **My platform tokens** $\rightarrow$ **Generate token**. | Assigned platform user policies & Grail scopes. |
| **API Access Token (Classic - New Format)** | `dt0c01` | Service accounts for Environment API v1, Settings API v2, and SCIM account integration. | Web UI: **Settings** $\rightarrow$ **Integration** $\rightarrow$ **Access tokens** $\rightarrow$ **Generate new token**. | Environment Admin / Change monitoring settings. |
| **API Access Token (Classic - Legacy Format)** | *(No prefix)* | Legacy unformatted token format used in older Dynatrace deployments. | Generated when *Create Dynatrace API tokens in the new format* toggle is OFF. | Environment Admin / Change monitoring settings. |
| **Personal Access Token (PAT)** | `dt0s01` / `dt0c01` | API access bound to an individual user's specific permissions and Management Zones. | Web UI: User Profile $\rightarrow$ **Personal access tokens** $\rightarrow$ **Generate token**. | Inherits creator's explicit user permissions. |
| **ActiveGate / Connection Token** | `dt0a01` | Establishes secure communication between ActiveGates, OneAgents, AWS Lambda layers, or OTel collectors and the cluster. | Auto-generated during ActiveGate setup or fetched via Deployment API. | ActiveGate Management permissions. |
| **OAuth2 Client Credentials** | `dt0s02`, `dt0s03`, `dt0s08` | Service-to-service authentication for Dynatrace Apps and Account Management APIs. | Dynatrace Developer Portal or Account Management $\rightarrow$ **OAuth Clients**. | Account Administrator / App permissions. |
| **OAuth2 Refresh Token** | `dt0s06` | Used by OAuth client flows to exchange for short-lived access tokens. | Issued dynamically during OAuth authorization exchange. | N/A (Flow-dependent). |

---

## Deep Dive by Token Category

### 1. Platform Tokens (`dt0s16`)
* **When to use:** Use when writing modern automation scripts that query Grail via DQL, execute workflows in Automation Engine, or send telemetry through OpenPipeline endpoints.
* **Header format:**
  ```http
  Authorization: Bearer dt0s16.XXXXXXXXXXXXXXXX.YYYYYYYYYYYYYYYY
  ```

### 2. API Tokens / Settings API v2 (`dt0c01`)
* **When to use:** Use for Infrastructure-as-Code (Terraform, Monaco), CI/CD pipelines, or updating Settings API schemas (such as `builtin:tags.auto-tagging`).
* **Header format:**
  ```http
  Authorization: Api-Token dt0c01.XXXXXXXXXXXXXXXX.YYYYYYYYYYYYYYYY
  ```

### 3. Connection Tokens (`dt0a01`)
* **When to use:** Stored in environment variables (like `DT_CONNECTION_AUTH_TOKEN`) for serverless functions (AWS Lambda), OpenTelemetry collectors, or ActiveGate configuration.
* **Header format:**
  ```http
  Authorization: Api-Token dt0a01.XXXXXXXXXXXXXXXX.YYYYYYYYYYYYYYYY
  ```

---

## PowerShell Execution Examples

### Example A: Reading Auto-Tagging Configurations (Settings API v2)

```powershell
# Define environment variables
$TenantUrl = "https://{your-environment-id}.live.dynatrace.com"
$ApiToken  = $env:DT_CONNECTION_AUTH_TOKEN  # Uses ActiveGate/API token stored in env

$Headers = @{
    "Authorization" = "Api-Token $ApiToken"
    "Content-Type"  = "application/json"
}

# Fetch auto-tagging rule definitions
$Url = "$TenantUrl/api/v2/settings/objects?schemaIds=builtin:tags.auto-tagging"
$Response = Invoke-RestMethod -Uri $Url -Method Get -Headers $Headers

# Output rule names and object IDs
$Response.items | Select-Object objectId, @{Name="RuleName"; Expression={$_.value.name}}, author
```

### Example B: Querying Applied Tags on Hosts (Entities API v2)

```powershell
$EntityType = 'type("HOST")'
$Url = "$TenantUrl/api/v2/entities?entitySelector=$EntityType&fields=tags"

$Response = Invoke-RestMethod -Uri $Url -Method Get -Headers $Headers

# Flatten and format host-to-tag relationships
$Response.entities | ForEach-Object {
    $HostName = $_.displayName
    foreach ($tag in $_.tags) {
        [PSCustomObject]@{
            HostName = $HostName
            TagKey   = $tag.key
            TagValue = $tag.value
            Context  = $tag.context
        }
    }
} | Format-Table -AutoSize
```

---

## Security Best Practices

1. **Least Privilege Principle:** When generating API access tokens, assign only the specific required scopes (e.g., `settings.read`, `entities.read`) rather than full environment access.
2. **Use PATs for Personal Scripts:** Never use generic administrative environment API tokens for personal utility scripts.
3. **Environment Variable Storage:** Store tokens in process environment variables (e.g., `$env:DT_CONNECTION_AUTH_TOKEN`) or secret managers rather than hardcoding credentials into scripts.
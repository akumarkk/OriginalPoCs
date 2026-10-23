$TenantUrl = "https://{your-environment-id}.live.dynatrace.com"
$ApiToken  = $env:DT_API_TOKEN_TAGS  # or replace with "YOUR_API_TOKEN"
$Headers   = @{
    "Authorization" = "Api-Token $ApiToken"
    "Content-Type"  = "application/json"
}

# Requires token scope: settings.read
$Url = "$TenantUrl/api/v2/settings/objects?schemaIds=builtin:tags.auto-tagging"

$Response = Invoke-RestMethod -Uri $Url -Method Get -Headers $Headers

#$Response.items | Select-Object -ExpandProperty objectId
# Output full details for each rule formatted as structured JSON
$Response.items | ConvertTo-Json -Depth 10
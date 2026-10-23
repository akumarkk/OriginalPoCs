
$TenantUrl = "https://{env}.apps.dynatrace.com/platform/classic/environment-api/v2"
$ApiToken = "dt0s16."
$Headers = @{ "Authorization" = "Bearer $ApiToken" }

$Body = ConvertTo-Json -Depth 10 -InputObject @(
    @{
        schemaId = "builtin:tags.auto-tagging"
        scope    = "environment"
        value    = @{
            name  = "EnvironmentTag"
            rules = @(
                @{
                    enabled            = $true
                    type               = "SELECTOR"
                    valueNormalization = "Leave text as-is"
                    valueFormat        = "{HOST:Environment}"
                    entitySelector     = "type(HOST),tag(Production)"
                }
            )
        }
    }
)

$Url = "$TenantUrl/settings/objects"
$Result = Invoke-RestMethod -Uri $Url -Method Post -Headers $Headers -ContentType "application/json" -Body $Body
$Result
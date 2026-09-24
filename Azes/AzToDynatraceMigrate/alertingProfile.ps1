$EnvId = ""
$ApiToken = ""

# Fallback check if environment variables aren't set in your current terminal session
if (-not $EnvId -or -not $ApiToken) {
    Write-Host "Please ensure DYNATRACE_ENV_ID and DYNATRACE_API_TOKEN are set in your environment." -ForegroundColor Red
    exit
}

$Uri = "https://$EnvId.live.dynatrace.com/api/v2/settings/objects"

$Headers = @{
    "Accept"        = "application/json"
    "Content-Type"  = "application/json; charset=utf-8"
    "Authorization" = "Api-Token $ApiToken"
}

# Payload to create an instant alerting profile for errors and custom metric events
$Body = @"
[
  {
    "schemaId": "builtin:alerting.profile",
    "scope": "environment",
    "value": {
      "name": "Travel-Instant-Alerting-Profile",
      "severityRules": [
        {
          "delayInMinutes": 0,
          "severityLevel": "ERRORS",
          "tagFilterIncludeMode": "NONE",
          "tagFilter": []
        },
        {
          "delayInMinutes": 0,
          "severityLevel": "CUSTOM_ALERT",
          "tagFilterIncludeMode": "NONE",
          "tagFilter": []
        }
      ]
    }
  }
]
"@

# Bypass SSL certificate trust check if needed in corporate environments
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

try {
    $Response = Invoke-RestMethod -Uri $Uri -Method Post -Headers $Headers -Body $Body
    Write-Host "Successfully created Alerting Profile!" -ForegroundColor Green
    $Response | Format-List
}
catch {
    Write-Host "Failed to create alerting profile: $_" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        $_.ErrorDetails.Message
    }
}
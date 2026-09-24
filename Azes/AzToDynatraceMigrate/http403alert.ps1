$EnvId = "traveldyn"
$ApiToken = $env:DYN_API_TOKEN

$Uri = "https://$EnvId.live.dynatrace.com/api/v2/settings/objects"

$Headers = @{
    "Accept"        = "application/json; charset=utf-8"
    "Content-Type"  = "application/json; charset=utf-8"
    "Authorization" = "Api-Token $ApiToken"
}

# Bypass SSL certificate trust check
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

# Define the JSON body using PowerShell here-string or hashtables converted to JSON
$Body = @"
[
  {
    "schemaId": "builtin:anomaly-detection.metric-events",
    "scope": "environment",
    "value": {
      "enabled": true,
      "summary": "Http403 > threshold for travel-test-func-westus2",
      "queryDefinition": {
        "type": "METRIC_SELECTOR",
        "metricSelector": "builtin:service.errors.server.status403:filter(eq(\"service.name\", \"intime-test-func-westus2\"))"
      },
      "modelProperties": {
        "type": "STATIC_THRESHOLD",
        "alertCondition": "ABOVE",
        "threshold": 3,
        "violatingSamples": 3,
        "samples": 5,
        "dealertingSamples": 5,
        "alertOnNoData": false
      },
      "eventTemplate": {
        "title": "Http403 > 3 on travel-test-func-westus2",
        "description": "The HTTP 403 error count has exceeded the threshold of 3.",
        "eventType": "CUSTOM_ALERT",
        "davisMerge": true,
        "metadata": []
      }
    }
  }
]
"@

Write-Host "Request Body: $Body" -ForegroundColor Cyan

try {
    $Response = Invoke-RestMethod -Uri $Uri -Method Post -Headers $Headers -Body $Body
    Write-Host "Successfully created Dynatrace alert!" -ForegroundColor Green
    $Response | Format-List
}
catch {
    Write-Host "Failed to create alert: $_" -ForegroundColor Red
    $_.ErrorDetails.Message
}
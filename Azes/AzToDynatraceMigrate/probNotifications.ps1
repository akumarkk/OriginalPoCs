$EnvId = ""
$ApiToken = ""
$Uri = "https://$EnvId.live.dynatrace.com/api/v2/settings/objects"

$Headers = @{
    "Accept"        = "application/json"
    "Content-Type"  = "application/json; charset=utf-8"
    "Authorization" = "Api-Token $ApiToken"
}

# This schema configures the email recipient destination and maps it to your alerting profile
$Body = @"
[
  {
    "schemaId": "builtin:problem.notifications",
    "scope": "environment",
    "value": {
      "enabled": true,
      "type": "EMAIL",
      "displayName": "tra-Email-Alerts",
      "alertingProfile": "xyzlsdGluOmFsZXJ0aW5nLnByb2ZpbGUABnRlbmFudAAGdGVuYW50ACQ0MDY5NmFjNS0yODM2LTM5MTgtOGJhYy1iZGU3MDU3OTU1OWa-71TeFdrerQ",
      "emailNotification": {
        "recipients": [],
        "ccRecipients": [],
        "bccRecipients": [],
        "subject": "ALERT: {ImpactedEntity} - HTTP 403 Error Triggered",
        "body": "An HTTP 403 error has been detected on travel-test-func-westus2.\n\nDetails:\n{ImpactedEntities}",
        "notifyClosedProblems": true
      }
    }
  }
]
"@

[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

try {
    $Response = Invoke-RestMethod -Uri $Uri -Method Post -Headers $Headers -Body $Body
    Write-Host "Successfully created Dynatrace Email Notification!" -ForegroundColor Green
    $Response | Format-List
}
catch {
    Write-Host "Failed to create email notification: $_" -ForegroundColor Red
    if ($_.ErrorDetails.Message) { $_.ErrorDetails.Message }
}
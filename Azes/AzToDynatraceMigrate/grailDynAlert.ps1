# ==========================================
# Dynatrace DQL Alert Creation Script
# ==========================================

# 1. Configuration Variables
$TenantId    = ""
$TenantUrl   = "https://$TenantId.apps.dynatrace.com"
$ApiToken    = "dt0s16.AAZCPDYP."
$AppName     = ""
$AlertTitle  = "$AppName High HTTP 4xx Error Count Alert"
$Threshold   = 50  # Adjust violation threshold as needed

# 2. DQL Query for 4xx HTTP Errors
# Connects events to entities using dt.smartscape_source.id
$DqlQuery = @"
timeseries { fourXX = sum(dt.service.request.count, default: 0.0) }, filter: { http.response.status_code >= 400 and http.response.status_code < 500 }, by: { dt.smartscape_source.id }, interval: 1m
"@

# 3. Build API Payload
$Payload = @(
    @{
        schemaId = "builtin:davis.anomaly-detectors"
        scope    = "environment"
        value    = @{
            enabled           = $true
            title             = $AlertTitle
            description       = "$AppName Automated alert for HTTP 4xx client errors exceeding threshold."
            source            = "DQL_Custom_Alert"
            executionSettings = @{
                queryOffset = 1
            }
            analyzer          = @{
                name  = "dt.statistics.ui.anomaly_detection.StaticThresholdAnomalyDetectionAnalyzer"
                input = @(
                    @{ key = "query";              value = $DqlQuery }
                    @{ key = "threshold";          value = "$Threshold" }
                    @{ key = "alertCondition";     value = "ABOVE" }
                    @{ key = "slidingWindow";      value = "5" }
                    @{ key = "violatingSamples";   value = "3" }
                    @{ key = "dealertingSamples";  value = "5" }
                    @{ key = "alertOnMissingData"; value = "false" }
                )
            }
            eventTemplate     = @{
                properties = @(
                    @{ key = "event.name"; value = "$AppName High HTTP 4xx Error Rate Detected" },
                    @{ key = "event.type"; value = "CUSTOM_ALERT" }
                )
            }
        }
    }
)

# Serialize as a JSON array. In Windows PowerShell 5.1 a single-element array is
# unwrapped to a bare object by ConvertTo-Json, which the Settings API rejects.
$Payload = $Payload | ConvertTo-Json -Depth 10
if ($Payload.TrimStart() -notlike '`[*') {
    $Payload = "[$Payload]"
}

# 4. API Endpoint & Headers
$Uri = "$TenantUrl/platform/classic/environment-api/v2/settings/objects"
$Headers = @{
    "Authorization" = "Bearer $ApiToken"
    "Content-Type"  = "application/json"
}

# 5. Execute Request
try {
    Write-Host "Sending request to Dynatrace Settings API..." -ForegroundColor Cyan
    $Response = Invoke-RestMethod -Uri $Uri -Method Post -Headers $Headers -Body ([System.Text.Encoding]::UTF8.GetBytes($Payload))
    
    Write-Host "Alert successfully created!" -ForegroundColor Green
    Write-Host "Created Object ID: $($Response[0].objectId)" -ForegroundColor Yellow
}
catch {
    Write-Host "Failed to create alert." -ForegroundColor Red
    Write-Host "Error Details: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $Reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        Write-Host "API Response: $($Reader.ReadToEnd())" -ForegroundColor Red
    }
}
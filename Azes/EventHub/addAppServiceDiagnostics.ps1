param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory=$true)]
    [string]$NamespaceName,

    [Parameter(Mandatory=$true)]
    [string]$AppServiceName,

    [Parameter(Mandatory=$false)]
    [string]$AppServiceResourceGroup = $ResourceGroup,

    [Parameter(Mandatory=$false)]
    [string]$EventHubName = "fo-dynatrace-appservice-logs",

    [Parameter(Mandatory=$false)]
    [string]$AuthRuleName = "DiagnosticSettingsSendRule",

    [Parameter(Mandatory=$false)]
    [ValidateSet("Send", "Listen", "Manage")]
    [string[]]$Rights = @("Send"),

    [Parameter(Mandatory=$false)]
    [string]$DiagnosticSettingName = "send-logs-to-eventhub"
)


# Not "Stop": in PS 5.1 native stderr (even with 2>$null) becomes a terminating error; exit codes are checked explicitly.
$ErrorActionPreference = "Continue"

function Assert-Az {
    param([string]$Message)
    if ($LASTEXITCODE -ne 0) { throw $Message }
}

# 1. Validate the existing namespace, event hub, and app service
az eventhubs namespace show --resource-group $ResourceGroup --name $NamespaceName -o none 2>$null
Assert-Az "Namespace '$NamespaceName' not found in resource group '$ResourceGroup'."

az eventhubs eventhub show --resource-group $ResourceGroup --namespace-name $NamespaceName --name $EventHubName -o none 2>$null
Assert-Az "Event Hub '$EventHubName' not found in namespace '$NamespaceName'."

$appId = az webapp show --resource-group $AppServiceResourceGroup --name $AppServiceName --query id -o tsv 2>$null
Assert-Az "App Service '$AppServiceName' not found in resource group '$AppServiceResourceGroup'."
Write-Host "Validated namespace, event hub, and app service." -ForegroundColor Green

# 2. Create the namespace-level authorization rule if it doesn't exist (diagnostic settings require a namespace rule)
az eventhubs namespace authorization-rule show --resource-group $ResourceGroup --namespace-name $NamespaceName --name $AuthRuleName -o none 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Authorization Rule $AuthRuleName already exists, skipping." -ForegroundColor Yellow
} else {
    Write-Host "Creating Authorization Rule ($AuthRuleName) with rights: $($Rights -join ', ')..." -ForegroundColor Cyan
    az eventhubs namespace authorization-rule create `
      --resource-group $ResourceGroup `
      --namespace-name $NamespaceName `
      --name $AuthRuleName `
      --rights $Rights -o none
    Assert-Az "Failed to create authorization rule '$AuthRuleName'."
}

$ruleId = az eventhubs namespace authorization-rule show `
    --resource-group $ResourceGroup `
    --namespace-name $NamespaceName `
    --name $AuthRuleName `
    --query id -o tsv
Assert-Az "Failed to read authorization rule ID."

# 3. Create (or update) the App Service diagnostic setting streaming all log categories to the event hub
$logsFile = Join-Path ([IO.Path]::GetTempPath()) "eh_diag_logs_$([guid]::NewGuid()).json"
Set-Content -Path $logsFile -Value '[{"categoryGroup":"allLogs","enabled":true}]' -Encoding ASCII

try {
    Write-Host "Creating diagnostic setting '$DiagnosticSettingName' on $AppServiceName..." -ForegroundColor Cyan
    az monitor diagnostic-settings create `
      --name $DiagnosticSettingName `
      --resource $appId `
      --event-hub $EventHubName `
      --event-hub-rule $ruleId `
      --logs "@$logsFile" -o none
    Assert-Az "Failed to create diagnostic setting '$DiagnosticSettingName'."
} finally {
    Remove-Item $logsFile -Force -ErrorAction SilentlyContinue
}

Write-Host "Done. App Service '$AppServiceName' now streams logs to Event Hub '$EventHubName' via rule '$AuthRuleName'." -ForegroundColor Green

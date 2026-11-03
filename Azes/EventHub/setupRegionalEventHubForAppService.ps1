param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory=$true)]
    [string]$AppServiceName,

    [Parameter(Mandatory=$false)]
    [string]$AppServiceResourceGroup = $ResourceGroup,

    [Parameter(Mandatory=$false)]
    [string]$EventHubName = "fo-dynatrace-appservice-logs",

    [Parameter(Mandatory=$false)]
    [string]$NamespaceName
)

# Diagnostic settings require the Event Hubs namespace to be in the same region as the app.
$location = az webapp show --resource-group $AppServiceResourceGroup --name $AppServiceName --query location -o tsv 2>$null
if ($LASTEXITCODE -ne 0 -or -not $location) {
    throw "App Service '$AppServiceName' not found in resource group '$AppServiceResourceGroup'."
}
Write-Host "App Service region: $location" -ForegroundColor Cyan

if (-not $NamespaceName) {
    $NamespaceName = "fo-dynatrace-logs-ns-" + ($location -replace '\s', '').ToLower()
}

& "$PSScriptRoot\eventHub.ps1" -ResourceGroup $ResourceGroup -Location $location -NamespaceName $NamespaceName -EventHubName $EventHubName

& "$PSScriptRoot\addAppServiceDiagnostics.ps1" `
    -ResourceGroup $ResourceGroup `
    -NamespaceName $NamespaceName `
    -EventHubName $EventHubName `
    -AppServiceName $AppServiceName `
    -AppServiceResourceGroup $AppServiceResourceGroup `
    -AuthRuleName "LogForwarderAuthRule"

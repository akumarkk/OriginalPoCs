param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroup = "your-resource-group-name",

    [Parameter(Mandatory=$false)]
    [string]$Location = "eastus",

    [Parameter(Mandatory=$false)]
    [string]$NamespaceName = "fo-dynatrace-logs-ns-$(Get-Random)",

    [Parameter(Mandatory=$false)]
    [string]$EventHubName = "fo-dynatrace-appservice-logs"
)

# 1. Create the Event Hubs Namespace if it doesn't exist
az eventhubs namespace show --resource-group $ResourceGroup --name $NamespaceName -o none 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Namespace $NamespaceName already exists, skipping." -ForegroundColor Yellow
} else {
    Write-Host "Creating Event Hubs Namespace: $NamespaceName..." -ForegroundColor Cyan
    az eventhubs namespace create `
      --resource-group $ResourceGroup `
      --name $NamespaceName `
      --location $Location `
      --sku Standard
}

# 2. Create the Event Hub instance if it doesn't exist
az eventhubs eventhub show --resource-group $ResourceGroup --namespace-name $NamespaceName --name $EventHubName -o none 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Event Hub $EventHubName already exists, skipping." -ForegroundColor Yellow
} else {
    Write-Host "Creating Event Hub instance: $EventHubName..." -ForegroundColor Cyan
    az eventhubs eventhub create `
      --resource-group $ResourceGroup `
      --namespace-name $NamespaceName `
      --name $EventHubName `
      --partition-count 2
}

# 3. Create the Authorization Rule (SAS Policy) if it doesn't exist
$AuthRuleName = "LogForwarderAuthRule"
az eventhubs namespace authorization-rule show --resource-group $ResourceGroup --namespace-name $NamespaceName --name $AuthRuleName -o none 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Authorization Rule $AuthRuleName already exists, skipping." -ForegroundColor Yellow
} else {
    Write-Host "Creating Authorization Rule ($AuthRuleName)..." -ForegroundColor Cyan
    az eventhubs namespace authorization-rule create `
      --resource-group $ResourceGroup `
      --namespace-name $NamespaceName `
      --name $AuthRuleName `
      --rights Send Listen
}

Write-Host "Event Hub setup complete! Namespace: $NamespaceName, Event Hub: $EventHubName" -ForegroundColor Green
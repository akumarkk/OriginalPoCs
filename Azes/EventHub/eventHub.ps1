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

# 1. Create the Event Hubs Namespace
Write-Host "Creating Event Hubs Namespace: $NamespaceName..." -ForegroundColor Cyan
az eventhubs namespace create `
  --resource-group $ResourceGroup `
  --name $NamespaceName `
  --location $Location `
  --sku Standard

# 2. Create the Event Hub instance inside the namespace
Write-Host "Creating Event Hub instance: $EventHubName..." -ForegroundColor Cyan
az eventhubs eventhub create `
  --resource-group $ResourceGroup `
  --namespace-name $NamespaceName `
  --name $EventHubName `
  --partition-count 2 `
  --message-retention 1

# 3. Create an Authorization Rule (SAS Policy) for sending and listening
Write-Host "Creating Authorization Rule (LogForwarderAuthRule)..." -ForegroundColor Cyan
az eventhubs namespace authorization-rule create `
  --resource-group $ResourceGroup `
  --namespace-name $NamespaceName `
  --name "LogForwarderAuthRule" `
  --rights Send Listen

Write-Host "Event Hub setup complete! Namespace: $NamespaceName, Event Hub: $EventHubName" -ForegroundColor Green
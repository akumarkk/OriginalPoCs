param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory=$true)]
    [string]$NamespaceName,

    [Parameter(Mandatory=$false)]
    [string]$EventHubName = "fo-dynatrace-appservice-logs",

    [Parameter(Mandatory=$false)]
    [string]$AuthRuleName = "LogForwarderAuthRule",

    [Parameter(Mandatory=$false)]
    [string]$ConsumerGroup = "`$Default",

    [Parameter(Mandatory=$false)]
    [int]$ReceiveTimeoutSeconds = 30
)

$ErrorActionPreference = "Stop"

function Assert-Az {
    if ($LASTEXITCODE -ne 0) { throw "Azure CLI command failed (exit code $LASTEXITCODE)." }
}

# 1. Validate existence of namespace, event hub, and auth rule
Write-Host "Validating resources..." -ForegroundColor Cyan

az eventhubs namespace show --resource-group $ResourceGroup --name $NamespaceName -o none 2>$null
if ($LASTEXITCODE -ne 0) { throw "Namespace '$NamespaceName' not found in resource group '$ResourceGroup'." }
Write-Host "  [OK] Namespace: $NamespaceName" -ForegroundColor Green

az eventhubs eventhub show --resource-group $ResourceGroup --namespace-name $NamespaceName --name $EventHubName -o none 2>$null
if ($LASTEXITCODE -ne 0) { throw "Event Hub '$EventHubName' not found in namespace '$NamespaceName'." }
Write-Host "  [OK] Event Hub: $EventHubName" -ForegroundColor Green

az eventhubs eventhub consumer-group show --resource-group $ResourceGroup --namespace-name $NamespaceName --eventhub-name $EventHubName --name $ConsumerGroup -o none 2>$null
if ($LASTEXITCODE -ne 0) { throw "Consumer group '$ConsumerGroup' not found on Event Hub '$EventHubName'." }
Write-Host "  [OK] Consumer group: $ConsumerGroup" -ForegroundColor Green

az eventhubs namespace authorization-rule show --resource-group $ResourceGroup --namespace-name $NamespaceName --name $AuthRuleName -o none 2>$null
if ($LASTEXITCODE -ne 0) { throw "Authorization rule '$AuthRuleName' not found in namespace '$NamespaceName'." }
Write-Host "  [OK] Authorization rule: $AuthRuleName" -ForegroundColor Green

# 2. Fetch keys
$keys = az eventhubs namespace authorization-rule keys list `
    --resource-group $ResourceGroup `
    --namespace-name $NamespaceName `
    --name $AuthRuleName -o json | ConvertFrom-Json
Assert-Az

# 3. Send a test message via the REST API (SAS token auth)
$marker = [guid]::NewGuid().ToString()
$startTime = [DateTime]::UtcNow.AddSeconds(-5)
$body = @{ test = "eventhub-validation"; id = $marker; sentUtc = [DateTime]::UtcNow.ToString("o") } | ConvertTo-Json -Compress

$resourceUri = "https://$NamespaceName.servicebus.windows.net/$EventHubName"
$expiry = [DateTimeOffset]::UtcNow.AddMinutes(10).ToUnixTimeSeconds()
$encodedUri = [uri]::EscapeDataString($resourceUri)
$hmac = New-Object System.Security.Cryptography.HMACSHA256
$hmac.Key = [Text.Encoding]::UTF8.GetBytes($keys.primaryKey)
$signature = [Convert]::ToBase64String($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes("$encodedUri`n$expiry")))
$sasToken = "SharedAccessSignature sr=$encodedUri&sig=$([uri]::EscapeDataString($signature))&se=$expiry&skn=$AuthRuleName"

Write-Host "Sending test message (id=$marker)..." -ForegroundColor Cyan
Invoke-RestMethod -Method Post -Uri "$resourceUri/messages" `
    -Headers @{ Authorization = $sasToken } `
    -ContentType "application/atom+xml;type=entry;charset=utf-8" `
    -Body $body | Out-Null
Write-Host "  [OK] Message sent." -ForegroundColor Green

# 4. Receive via AMQP (REST API has no receive); uses the azure-eventhub Python SDK
$python = Get-Command python -ErrorAction SilentlyContinue
if (-not $python) { $python = Get-Command py -ErrorAction SilentlyContinue }
if (-not $python) { throw "Python is required to receive messages (REST API supports send only)." }

& $python.Source -c "import azure.eventhub" 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Installing azure-eventhub Python package..." -ForegroundColor Yellow
    & $python.Source -m pip install --quiet azure-eventhub
    Assert-Az
}

$receiver = @'
import os, sys, threading
from datetime import datetime, timezone
from azure.eventhub import EventHubConsumerClient

marker = os.environ["EH_MARKER"]
timeout = int(os.environ["EH_TIMEOUT"])
start = datetime.fromisoformat(os.environ["EH_START"]).replace(tzinfo=timezone.utc)
found = threading.Event()

client = EventHubConsumerClient.from_connection_string(
    os.environ["EH_CONN"],
    consumer_group=os.environ["EH_GROUP"],
    eventhub_name=os.environ["EH_NAME"],
)

def on_event(ctx, event):
    if event is not None and marker in event.body_as_str():
        print("Received: " + event.body_as_str())
        found.set()

def run():
    with client:
        client.receive(on_event=on_event, starting_position=start)

t = threading.Thread(target=run, daemon=True)
t.start()
ok = found.wait(timeout)
client.close()
sys.exit(0 if ok else 1)
'@

$tmp = Join-Path ([IO.Path]::GetTempPath()) "eh_receive_$marker.py"
Set-Content -Path $tmp -Value $receiver -Encoding UTF8

$env:EH_CONN = $keys.primaryConnectionString
$env:EH_NAME = $EventHubName
$env:EH_GROUP = $ConsumerGroup
$env:EH_MARKER = $marker
$env:EH_TIMEOUT = $ReceiveTimeoutSeconds
$env:EH_START = $startTime.ToString("yyyy-MM-ddTHH:mm:ss")

Write-Host "Waiting up to $ReceiveTimeoutSeconds s to receive the message..." -ForegroundColor Cyan
try {
    & $python.Source $tmp
    $received = ($LASTEXITCODE -eq 0)
} finally {
    Remove-Item $tmp -Force -ErrorAction SilentlyContinue
    Remove-Item Env:EH_CONN, Env:EH_NAME, Env:EH_GROUP, Env:EH_MARKER, Env:EH_TIMEOUT, Env:EH_START -ErrorAction SilentlyContinue
}

if ($received) {
    Write-Host "  [OK] Send/receive validation succeeded." -ForegroundColor Green
} else {
    Write-Host "  [FAIL] Message was not received within $ReceiveTimeoutSeconds s." -ForegroundColor Red
    exit 1
}

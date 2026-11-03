<#
.SYNOPSIS
Verifies OIDC client ID/secret entries exist in a Key Vault and have a non-empty value.

.EXAMPLE
./Verify-KvSecrets.ps1 -SubscriptionId <sub> -ResourceGroup <rg> -VaultName <kv>
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $SubscriptionId,
    [Parameter(Mandatory)] [string] $ResourceGroup,
    [Parameter(Mandatory)] [string] $VaultName
)

$ErrorActionPreference = 'Stop'

$functions = '', ''
$names = foreach ($kind in 'ClientID', 'ClientSecret') { foreach ($f in $functions) { "Prefix-$kind-$f" } }

if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI (az) not found.'
}

az account set --subscription $SubscriptionId
if ($LASTEXITCODE -ne 0) { throw "Cannot select subscription '$SubscriptionId'." }

# Confirm the vault exists in the given resource group
az keyvault show --name $VaultName --resource-group $ResourceGroup --query name -o tsv 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Key Vault '$VaultName' not found in resource group '$ResourceGroup'." }

$results = foreach ($name in $names) {
    $value = az keyvault secret show --vault-name $VaultName --name $name --query value -o tsv 2>$null
    $status = if ($LASTEXITCODE -ne 0) { 'MISSING' }
              elseif ([string]::IsNullOrEmpty($value)) { 'EMPTY' }
              else { 'OK' }
    [pscustomobject]@{ Secret = $name; Status = $status }
}

$results | Format-Table -AutoSize

$failed = @($results | Where-Object Status -ne 'OK')
if ($failed.Count) {
    Write-Host "$($failed.Count) of $($results.Count) checks failed." -ForegroundColor Red
    exit 1
}
Write-Host "All $($results.Count) secrets exist with values." -ForegroundColor Green

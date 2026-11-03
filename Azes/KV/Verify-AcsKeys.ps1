<#
.SYNOPSIS
Verifies config keys exist in an Azure App Configuration store.

.EXAMPLE
./Verify-AcsKeys.ps1 -SubscriptionId <sub> -ResourceGroup <rg> -StoreName <acs> -Keys 'App:Setting1','App:Setting2'
./Verify-AcsKeys.ps1 -SubscriptionId <sub> -ResourceGroup <rg> -StoreName <acs> -Keys (Get-Content keys.txt) -Label prod
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $SubscriptionId,
    [Parameter(Mandatory)] [string] $ResourceGroup,
    [Parameter(Mandatory)] [string] $StoreName,
    [Parameter(Mandatory)] [string[]] $Keys,
    [string] $Label
)

# 'Stop' turns az stderr into a terminating error on PS 5.1; exit codes are checked explicitly.
$ErrorActionPreference = 'Continue'

if (-not (Get-Command az -ErrorAction SilentlyContinue)) { throw 'Azure CLI (az) not found.' }

az account set --subscription $SubscriptionId
if ($LASTEXITCODE -ne 0) { throw "Cannot select subscription '$SubscriptionId'." }

az appconfig show --name $StoreName --resource-group $ResourceGroup --query name -o tsv 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) { throw "App Configuration '$StoreName' not found in resource group '$ResourceGroup'." }

$results = foreach ($key in $Keys | Where-Object { $_ -and $_.Trim() } | ForEach-Object { $_.Trim() }) {
    # 'kv list' without --label matches the key under any label.
    $azArgs = @('appconfig', 'kv', 'list', '--name', $StoreName, '--key', $key, '--auth-mode', 'login', '--query', '[].label', '-o', 'json')
    if ($Label) { $azArgs += @('--label', $Label) }
    $json = az @azArgs 2>$null
    if ($LASTEXITCODE -ne 0) { throw "Failed to query key '$key' (check access to '$StoreName')." }
    $labels = @($json | ConvertFrom-Json)
    [pscustomobject]@{
        Key    = $key
        Status = if ($labels.Count) { 'OK' } else { 'MISSING' }
        Labels = ($labels | ForEach-Object { if ($_) { $_ } else { '(none)' } }) -join ','
    }
}

$results | Format-Table -AutoSize

$failed = @($results | Where-Object Status -ne 'OK')
if ($failed.Count) {
    Write-Host "$($failed.Count) of $($results.Count) keys missing." -ForegroundColor Red
    exit 1
}
Write-Host "All $($results.Count) keys exist." -ForegroundColor Green

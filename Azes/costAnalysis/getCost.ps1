$subs = @()

# Write query body to a file to avoid PowerShell/az rest argument mangling
$bodyPath = Join-Path $env:TEMP "costquery.json"
@'
{
  "type": "ActualCost",
  "timeframe": "MonthToDate",
  "dataset": {
    "aggregation": { "totalCost": { "name": "Cost", "function": "Sum" } }
  }
}
'@ | Set-Content -Path $bodyPath -Encoding ASCII

foreach ($sub in $subs) {
    Write-Output "=== $sub ==="
    $subId = az account show --subscription "$sub" --query id -o tsv
    if (-not $subId) {
        Write-Warning "Could not find subscription: $sub"
        continue
    }

    $raw = az rest --method POST `
        --url "https://management.azure.com/subscriptions/$subId/providers/Microsoft.CostManagement/query?api-version=2023-11-01" `
        --headers "Content-Type=application/json" `
        --body "@$bodyPath"

    if ($LASTEXITCODE -ne 0 -or -not $raw) {
        Write-Warning "Query failed for $sub"
        continue
    }

    $result  = $raw | ConvertFrom-Json
    $columns = $result.properties.columns.name
    $costIdx = [array]::IndexOf($columns, "Cost")

    $total    = ($result.properties.rows | ForEach-Object { $_[$costIdx] } | Measure-Object -Sum).Sum
    $currency = $result.properties.rows[0][[array]::IndexOf($columns, "Currency")]

    "{0}: {1:N2} {2} (MonthToDate)" -f $sub, $total, $currency
}
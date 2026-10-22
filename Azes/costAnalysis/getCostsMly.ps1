$subs = @("FlightOperations.PROD", "FlightOperations.QA", "FlightOperations.DEV.test")
$monthsBack = 3   # <-- how many past months to include (current MTD is also shown)

function Get-Cost {
    param([string]$SubId, [string]$From, [string]$To)
    $bodyPath = Join-Path $env:TEMP "costquery.json"
    @"
{
  "type": "ActualCost",
  "timeframe": "Custom",
  "timePeriod": { "from": "$From", "to": "$To" },
  "dataset": {
    "aggregation": { "totalCost": { "name": "Cost", "function": "Sum" } }
  }
}
"@ | Set-Content -Path $bodyPath -Encoding ASCII

    $raw = az rest --method POST `
        --url "https://management.azure.com/subscriptions/$SubId/providers/Microsoft.CostManagement/query?api-version=2023-11-01" `
        --headers "Content-Type=application/json" `
        --body "@$bodyPath"

    if ($LASTEXITCODE -ne 0 -or -not $raw) { return $null }

    $result  = $raw | ConvertFrom-Json
    $columns = $result.properties.columns.name
    $costIdx = [array]::IndexOf($columns, "Cost")
    $total   = ($result.properties.rows | ForEach-Object { $_[$costIdx] } | Measure-Object -Sum).Sum
    $currency = $result.properties.rows[0][[array]::IndexOf($columns, "Currency")]
    return @{ Total = $total; Currency = $currency }
}

foreach ($sub in $subs) {
    Write-Output "=== $sub ==="
    $subId = az account show --subscription "$sub" --query id -o tsv
    if (-not $subId) { Write-Warning "Could not find subscription: $sub"; continue }

    # Current month-to-date
    $mtd = Get-Cost $subId ("{0:yyyy-MM}-01" -f (Get-Date)) ("{0:yyyy-MM-dd}" -f (Get-Date))
    if ($mtd) { "{0,-15} {1,12:N2} {2}  (MonthToDate)" -f (Get-Date -Format "MMM yyyy"), $mtd.Total, $mtd.Currency }

    # Past months
    for ($i = 1; $i -le $monthsBack; $i++) {
        $month = (Get-Date).AddMonths(-$i)
        $from  = "{0:yyyy-MM}-01" -f $month
        $to    = "{0:yyyy-MM-dd}" -f $month.AddMonths(1).AddDays(-1)
        $cost  = Get-Cost $subId $from $to
        if ($cost) { "{0,-15} {1,12:N2} {2}" -f ($month.ToString("MMM yyyy")), $cost.Total, $cost.Currency }
        else       { Write-Warning "Query failed for $sub ($from to $to)" }
    }
}
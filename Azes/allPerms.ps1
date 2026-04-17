$id = "38037c43-bcf7-4bea-897a-c7a3445573c0"
$allSubs = Get-AzSubscription

foreach ($sub in $allSubs) {
    Set-AzContext -SubscriptionId $sub.Id -Force | Out-Null
    $assign = Get-AzRoleAssignment -ObjectId $id -ErrorAction SilentlyContinue
    if ($assign) {
        Write-Host "Found in Subscription: $($sub.Name)" -ForegroundColor Green
        $assign | Select-Object RoleDefinitionName, Scope
    }
}
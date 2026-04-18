$oldProgressPreference = $ProgressPreference
$ProgressPreference = 'SilentlyContinue'

$resourceName = ""
$slotName = "staging"
$subscriptionNames = @(
    
)

# Get all subscriptions that match your list
$allSubs = Get-AzSubscription | Where-Object { $_.Name -in $subscriptionNames }

foreach ($sub in $allSubs) {
    Set-AzContext -SubscriptionId $sub.Id -Force | Out-Null
    Write-Host "`n--- Checking Subscription: $($sub.Name) ---" -ForegroundColor Cyan

    # Find the Web App
    $resource = Get-AzWebApp -Name $resourceName -ErrorAction SilentlyContinue
    
    if ($resource) {
        # Find the specific Slot
        $slot = Get-AzWebAppSlot -ResourceGroupName $resource.ResourceGroup -Name $resourceName -Slot $slotName -ErrorAction SilentlyContinue
        
        if ($slot) {
            Write-Host "Found Slot: $($slot.Name)" -ForegroundColor Green
            
            # Identify the Managed Identity Object ID
            $principalId = $slot.Identity.PrincipalId

            if (-not $principalId) {
                Write-Host "Check Failed: This slot does not have a Managed Identity enabled." -ForegroundColor Red
            } else {
                Write-Host "Managed Identity ID: $principalId"
                
                # Retrieve role assignments where this Identity is the ASSIGNEE
                $assignments = Get-AzRoleAssignment -ObjectId $principalId -ErrorAction SilentlyContinue
                
                # Filter for App Configuration Data Reader
                $appConfigReaderAssignments = $assignments | Where-Object { $_.RoleDefinitionName -eq "App Configuration Data Reader" }

                if ($appConfigReaderAssignments) {
                    Write-Host "Found 'App Configuration Data Reader' role assignment(s):" -ForegroundColor Yellow
                    foreach ($assign in $appConfigReaderAssignments) {
                        
                        # Extract the Resource Name from the Scope string
                        # Scope usually looks like: /subscriptions/.../providers/Microsoft.AppConfiguration/configurationStores/STORE_NAME
                        $configStoreName = $assign.Scope.Split('/')[-1]
                        
                        Write-Host "  - App Configuration Store: $configStoreName"
                        Write-Host "    Scope: $($assign.Scope)"
                    }
                } else {
                    Write-Host "No 'App Configuration Data Reader' roles found assigned to this identity." -ForegroundColor Gray
                    
                    if ($assignments) {
                        Write-Host "Other roles held by this identity:"
                        $assignments | Select-Object RoleDefinitionName, Scope | Format-Table -AutoSize
                    }
                }
            }
        }
    } else {
        Write-Host "Resource '$resourceName' not found in this subscription." -ForegroundColor Gray
    }
}

$ProgressPreference = $oldProgressPreference
Write-Host "`nSearch Complete." -ForegroundColor White
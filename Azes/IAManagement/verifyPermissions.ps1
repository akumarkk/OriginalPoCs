[CmdletBinding()]
param (
    # --- Configuration ---
    [Parameter(Mandatory=$true)]
    [string]$groupName,

    [Parameter(Mandatory=$true)]
    [string]$identityName,
    
    [Parameter(Mandatory=$true)]
    [string]$identityResourceGroup,

    [Parameter(Mandatory=$true)]
    [string]$identitySubscriptionId,

    [Parameter(Mandatory=$true)]
    [string]$rgName,

    [Parameter(Mandatory=$true)]
    [string]$namespaceName,

    [Parameter(Mandatory=$true)]
    [string]$topicName,

    [Parameter(Mandatory=$true)]
    [string]$subscription,

    [Parameter(Mandatory=$true)]
    [string]$subscriptionId
)

# --- 1. Resolve IDs via AZ CLI ---
Write-Host "Resolving Identities..." -ForegroundColor Gray

# Set account context first so all resource lookups run in the intended subscription.
az account set --subscription $subscriptionId 2>$null
$activeSubId = az account show --query id -o tsv

if (-not $activeSubId) {
    Write-Host "Unable to resolve active subscription. Run 'az login' and retry." -ForegroundColor Red
    exit 1
}

if ($activeSubId -ne $subscriptionId) {
    Write-Host "Active subscription ($activeSubId) does not match expected $subscriptionId." -ForegroundColor Red
    exit 1
}

# Get Group ID
$groupId = az ad group show --group $groupName --query id -o tsv 2>$null

# Get managed identity principal ID from the identity resource (more reliable than display-name lookup)
$identityId = az identity show --resource-group $identityResourceGroup --name $identityName --query principalId -o tsv --subscription $identitySubscriptionId 2>$null

$idsToCheck = @()
if ($groupId) { $idsToCheck += [PSCustomObject]@{ Name=$groupName; ID=$groupId; Type="Group" } }
if ($identityId) { $idsToCheck += [PSCustomObject]@{ Name=$identityName; ID=$identityId; Type="Identity" } }

# --- 2. Define Scope ---
# Verify resource group exists in the expected subscription before scope checks.
$rgExists = az group exists --name $rgName --subscription $subscriptionId
if ($rgExists -ne "true") {
    # Fallback: try discovering the namespace's actual resource group in this subscription.
    $discoveredRg = az servicebus namespace list --subscription $subscriptionId --query "[?name=='$namespaceName'].resourceGroup | [0]" -o tsv 2>$null
    if ($discoveredRg) {
        $rgName = $discoveredRg
        Write-Host "Using discovered resource group for namespace: $rgName" -ForegroundColor DarkYellow
    } else {
        Write-Host "Resource group '$rgName' was not found in subscription '$subscriptionId'." -ForegroundColor Red
        exit 1
    }
}

$scope = "/subscriptions/$subscriptionId/resourceGroups/$rgName/providers/Microsoft.ServiceBus/namespaces/$namespaceName/topics/$topicName"

# --- 3. Execute Audit ---
Write-Host "Auditing access for Topic: $topicName`n" -ForegroundColor Cyan

foreach ($item in $idsToCheck) {
    Write-Host "Checking $($item.Type): $($item.Name)..." -ForegroundColor DarkCyan
    
    # az role assignment list returns JSON by default; ConvertFrom-Json makes it a PSObject
    $results = az role assignment list --assignee $item.ID --scope $scope --include-groups --subscription $subscriptionId | ConvertFrom-Json
    
    if ($results) {
        $results | Select-Object `
            @{N="PrincipalName"; E={$item.Name}},
            @{N="Role"; E={$_.roleDefinitionName}},
            @{N="ScopeLevel"; E={$_.scope.Split('/')[-1]}}, 
            @{N="Assignment"; E={if($_.principalId -eq $item.ID){"Direct"}else{"Inherited via Group"}}} | 
            Format-Table -AutoSize
    } else {
        Write-Host "   No access found for $($item.Name) at this level.`n" -ForegroundColor Yellow
    }
}
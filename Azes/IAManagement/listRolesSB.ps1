param (
    [string]$IdentityBaseName,
    [string[]]$Subs = @()
)

function ListRole {
    param(
        [string] $IdentityName,
        [string[]] $Subscriptions
    )

    # 2. Get the Principal ID once (tenant-wide lookup)
    Write-Host "Looking up Principal ID for: $IdentityName" -ForegroundColor Cyan
    $PrincipalId = az ad sp list --display-name $IdentityName --query "[0].id" -o tsv

    if (-not $PrincipalId) {
        Write-Error "Identity '$IdentityName' not found in Entra ID."
        return
    }
    Write-Host "Found Principal ID: $PrincipalId" -ForegroundColor Green

    # 3. Get all subscriptions
    #$Subscriptions = az account list --query "[].id" -o tsv

    # 4. Iterate and list assignments
    foreach ($SubId in $Subscriptions) {
        Write-Host "--- Checking Subscription: $SubId ---" -ForegroundColor Yellow
    
        # Temporarily set context to the subscription
        az account set --subscription $SubId
    
        # List assignments for the identity in this subscription
        # '--all' ensures we catch assignments inherited from RGs or the sub itself
        $Assignments = az role assignment list --assignee $PrincipalId --all --output table
    
        if ($Assignments) {
            $Assignments
        } else {
            Write-Host "No role assignments found for $IdentityName in subscription $SubId."
        }
    }
}

# 1. Configuration
#$BaseName = "fo-tfdmtarget"
$Environments = "test", "qa", "prod"

# Generate the list using a loop
$IdentityList = if ($IdentityName) {
    $IdentityName
} else {
    foreach ($Env in $Environments) {
        "{0}-{1}-identity" -f $IdentityBaseName, $Env
    }
}

foreach($IdName in $IdentityList){
    ListRole -IdentityName $IdName -Subscriptions $Subs
}
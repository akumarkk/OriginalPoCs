# 1. Configuration
$IdentityName = ""
$BaseName = ""
$Environments = "test", "qa", "prod"
$Subs =  @("85adbd32-c5a8-4897-b82d-92df3c40c5b4", 
"43603d42-edb7-4565-8725-4d6e487b5f45",
"de2f4084-7f76-406c-aadc-f089f07a603e"
    )

# Generate the list using a loop
$IdentityList = foreach ($Env in $Environments) {
    "{0}-{1}-identity" -f $BaseName, $Env
}

foreach($IdentityName in $IdentityList){
    ListRole($IdentityName, $Subs)
}

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
    foreach ($SubId in $Subs) {
        Write-Host "--- Checking Subscription: $SubId ---" -ForegroundColor Yellow
    
        # Temporarily set context to the subscription
        az account set --subscription $SubId
    
        # List assignments for the identity in this subscription
        # '--all' ensures we catch assignments inherited from RGs or the sub itself
        $Assignments = az role assignment list --assignee $PrincipalId --all --output table
    
        if ($Assignments) {
            $Assignments
        }
        else {
            Write-Host "No assignments found in this subscription." -ForegroundColor Gray
        }
    }
}
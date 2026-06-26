# Script to assign Azure Service Bus roles to a managed identity

# Variables for the managed identity
$principalId = ""

# Variables for the Service Bus Topic
$subscriptionId = ""
$resourceGroupName = "EventsDistributorTestgroup"
$namespaceName = "test-enterpriseevents-03"
$topicName = "as.events"

# Construct the scope for the role assignment
$scope = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroupName/providers/Microsoft.ServiceBus/namespaces/$namespaceName/topics/$topicName"

# Roles to assign
$roles = @(
    "Azure Service Bus Data Owner",
    "Azure Service Bus Data Receiver",
    "Azure Service Bus Data Sender"
)

# Login to Azure (uncomment if you are not already logged in)
# az login
# az account set --subscription $subscriptionId

# Loop through the roles and create the role assignments
foreach ($role in $roles) {
    Write-Host "Assigning role '$role' to principal '$principalId' at scope '$scope'..."
    az role assignment create --assignee $principalId --role $role --scope $scope
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully assigned role '$role'."
    } else {
        Write-Host "Failed to assign role '$role'."
    }
    Write-Host ""
}

Write-Host "All specified roles have been processed."

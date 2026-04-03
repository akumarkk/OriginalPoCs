az role assignment list `
    --assignee "id" `
    --scope "/subscriptions/<sub-id>/resourceGroups/EventsDistributor-Test-group/providers/Microsoft.ServiceBus/namespaces/<ns-name>/topics/<topic>" `
    --include-inherited `
    --output table
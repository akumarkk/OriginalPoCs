using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.ResourceManager;
using Azure.ResourceManager.ServiceBus;

// 1. Get configuration from environment variables (Set these in PowerShell first!)
string connectionString = Environment.GetEnvironmentVariable("SB_CONN_NAME");
string queueName = Environment.GetEnvironmentVariable("SB_QUEUE_NAME");

if (string.IsNullOrEmpty(connectionString)) {
    Console.WriteLine("Missing connection string!");
    return;
}

// --- PART A: Metadata (Printing RG and Subscription) ---
// We use the ArmClient to look up resource details
var armClient = new ArmClient(new DefaultAzureCredential());

// Parse the namespace name from the connection string to find it in Azure
var properties = ServiceBusConnectionStringProperties.Parse(connectionString);
string namespaceName = properties.FullyQualifiedNamespace.Split('.')[0];

Console.WriteLine($"Checking Azure for Namespace: {namespaceName}...");

await foreach (var sub in armClient.GetSubscriptions().GetAllAsync())
{
    // Search for the namespace across your subscriptions
    await foreach (var sbNamespace in sub.GetServiceBusNamespacesAsync())
    {
        if (sbNamespace.Data.Name.Equals(namespaceName, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"SUB ID:         {sub.Data.SubscriptionId}");
            Console.WriteLine($"RESOURCE GROUP: {sbNamespace.Id.ResourceGroupName}");
            Console.WriteLine($"LOCATION:       {sbNamespace.Data.Location}");
            Console.WriteLine("--------------------------------------------------");
        }
    }
}

// --- PART B: Write to Queue ---
await using var client = new ServiceBusClient(connectionString);
var sender = client.CreateSender(queueName);

try 
{
    var message = new ServiceBusMessage("Recovery task: Tail assignment failure logic triggered.");
    await sender.SendMessageAsync(message);
    Console.WriteLine($"[Success] Message sent to {queueName}");
}
catch (Exception ex) 
{
    Console.WriteLine($"[Error] {ex.Message}");
}
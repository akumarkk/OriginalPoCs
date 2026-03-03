using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

class Program
{
    static async Task Main(string[] args)
    {
        // 1. Get your connection string from an environment variable (best practice)
        string connectionString = Environment.GetEnvironmentVariable("AppConfigConnectionString");

        var builder = new ConfigurationBuilder();

        // 2. Build the configuration provider
        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(connectionString)
                   // This tells the app to resolve Key Vault references
                   .ConfigureKeyVault(kv =>
                   {
                       kv.SetCredential(new DefaultAzureCredential());
                   })
                   .Select("anikris*");
        });

        IConfiguration config = builder.Build();
        foreach (var setting in config.AsEnumerable())
        {
            if (setting.Key.Contains("anikris"))
            {
                Console.WriteLine($"Key: {setting.Key} | Value: {setting.Value}");
            }
        }

        // 3. Access the value just like a normal configuration key
        
    }
}
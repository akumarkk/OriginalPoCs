using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

class Program
{
    // A static counter to see how many physical C# objects are created
    public static int InstancesCreated = 0;

    static async Task Main(string[] args)
    {
        const int totalSimulatedRequests = 10000;

        Console.WriteLine("=== TESTING DEFAULT AddDbContext ===");
        InstancesCreated = 0; // Reset counter
        
        var standardProvider = SetupStandardProvider();
        await SimulateRequestsAsync(standardProvider, totalSimulatedRequests);
        Console.WriteLine($"Total physical DbContext objects created: {InstancesCreated}\n");

        Console.WriteLine("=== TESTING AddDbContextPool ===");
        InstancesCreated = 0; // Reset counter
        
        var pooledProvider = SetupPooledProvider();
        await SimulateRequestsAsync(pooledProvider, totalSimulatedRequests);
        Console.WriteLine($"Total physical DbContext objects created: {InstancesCreated}\n");
    }

    // Standard Setup: Fresh instances every time
    private static IServiceProvider SetupStandardProvider()
    {
        var services = new ServiceCollection();
        services.AddDbContext<DemoDbContext>(options => 
            options.UseInMemoryDatabase("StandardDb"));
        return services.BuildServiceProvider();
    }

    // Pooled Setup: Reuses existing instances from the pool
    private static IServiceProvider SetupPooledProvider()
    {
        var services = new ServiceCollection();
        // Pool size defaults to 1024, but we can set it explicitly
        services.AddDbContextPool<DemoDbContext>(options => 
            options.UseInMemoryDatabase("PooledDb"), poolSize: 10); 
        return services.BuildServiceProvider();
    }

    // This simulates an API web request pipeline opening and closing scopes
    private static async Task SimulateRequestsAsync(IServiceProvider provider, int count)
    {
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
        {
            // Simulate a web request arriving (Creating a Scope)
            using (var scope = provider.CreateScope())
            {
                // Resolve the context inside this request
                var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                
                // Do a dummy database operation to force initialization
                await db.Database.EnsureCreatedAsync(); 
            } 
            // Web request ends here, scope is disposed
        }

        stopwatch.Stop();
        Console.WriteLine($"Processed {count} requests in {stopwatch.ElapsedMilliseconds}ms");
    }
}

// Custom DbContext that counts its own allocations
public class DemoDbContext : DbContext
{
    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
    {
        // Every time a new C# object instance is allocated in memory, increment this
        Interlocked.Increment(ref Program.InstancesCreated);
    }
}
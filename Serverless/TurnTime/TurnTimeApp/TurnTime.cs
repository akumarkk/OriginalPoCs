using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace TurnTimeApp;

public class TurnTime
{
    private readonly ILogger<TurnTime> _logger;

    public TurnTime(ILogger<TurnTime> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Function("HelloTurnTimeFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "TurnTimev1")] HttpRequest req, 
        FunctionContext executionContext)
    {
        var invocationId = executionContext.InvocationId;
        _logger.LogInformation("HelloTurnTimeFunction: HTTP trigger function processed a request.");
        _logger.LogInformation("Req: {Method}, {Path} {ActivityId}", req?.Method, req?.Path, Activity.Current?.Id ?? invocationId);

        var dtTenant = Environment.GetEnvironmentVariable("DT_TENANT");
        
        _logger.LogInformation("Dynatrace env vars: Tenant={Tenant}",
            dtTenant);

        var assembly = typeof(TurnTime).Assembly;
        var version = assembly.GetName().Version?.ToString() ?? "unknown";
        var assemblyName = assembly.GetName().Name ?? "TurnTimeApp";
        var currentTimeUtc = DateTime.UtcNow;

        return new OkObjectResult(new
        {
            message = "Welcome to Azure Functions!",
            assembly = assemblyName,
            version = version,
            dtTenant,
            currentTimeUtc
        });
    }
}

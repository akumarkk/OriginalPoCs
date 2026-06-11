using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        _logger.LogInformation("HelloTurnTimeFunction:  HTTP trigger function processed a request.");
        _logger.LogInformation("Req: {Property1}, {Property2} {Activityid}", req?.Method, req?.Path, Activity.Current?.Id ?? invocationId);
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}

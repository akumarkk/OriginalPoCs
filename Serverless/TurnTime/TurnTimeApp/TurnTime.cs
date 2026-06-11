using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TurnTimeApp;

public class TurnTime
{
    private readonly ILogger<TurnTime> _logger;

    public TurnTime(ILogger<TurnTime> logger)
    {
        _logger = logger;
    }

    [Function("TurnTime")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class TestFunction
{
    private readonly ILogger<TestFunction> _logger;

    public TestFunction(ILogger<TestFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessRequest")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("Processing an HTTP request.");

        string name = req.Query["name"];
        
        return new OkObjectResult($"Hello, {name ?? "World"}! This function executed successfully.");
    }
}
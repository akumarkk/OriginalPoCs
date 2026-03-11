using Microsoft.Extensions.Diagnostics.HealthChecks;

public class BingAircraftInfoApiHealthCheckService : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BingAircraftInfoApiHealthCheckService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
    {
        { "DatabaseConnection", "OK" },
        { "LatencyMs", 45 },
        { "Version", "1.2.0" }
    };

    var dataunh = new Dictionary<string, object>
        {
            { "ErrorCode", "PAYMENT_GATEWAY_TIMEOUT" },
            { "AttemptedUrl", "https://api.gateway.com" },
            
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            // Call a lightweight endpoint on the Aircraft API
            var response = await client.GetAsync("https://www.bing.com/travel/flights?form=UNKHUB&entrypoint=UNKHUB", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Aircraft API is up and running.", data);
            }

            dataunh["StatusCode"] = response.StatusCode;
            return HealthCheckResult.Unhealthy("Aircraft API returned an error code.", null, dataunh);
        }
        catch (Exception ex)
        {
            dataunh.Add("Exception", ex.Message);
            return HealthCheckResult.Unhealthy("Aircraft API is unreachable.", ex, dataunh);
        }
    }
}
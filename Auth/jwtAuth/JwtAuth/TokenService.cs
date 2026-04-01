using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class TokenIntrospectionService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public TokenIntrospectionService()
    {
        _httpClient = new HttpClient();

        // Reading from Environment Variables
        _endpoint = Environment.GetEnvironmentVariable("INTROSPECT_URL") 
            ?? throw new Exception("Missing INTROSPECT_URL env var.");
        _clientId = Environment.GetEnvironmentVariable("CLIENT_ID") 
            ?? throw new Exception("Missing CLIENT_ID env var.");
        _clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET") 
            ?? throw new Exception("Missing CLIENT_SECRET env var.");
    }

    public async Task<IntrospectionResponse?> IntrospectAsync(string token)
    {
        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("token", token),
            new KeyValuePair<string, string>("token_type_hint", "access_token")
        });

        // Basic Auth Header: base64(client_id:client_secret)
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        var response = await _httpClient.PostAsync(_endpoint, requestData);
        
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"content {content}");
        return JsonSerializer.Deserialize<IntrospectionResponse>(content);
    }
}

// Simple DTO to map the JSON response
public class IntrospectionResponse
{
    public bool active { get; set; }
    public string? scope { get; set; }
    public string? sub { get; set; }
    public long exp { get; set; }
}
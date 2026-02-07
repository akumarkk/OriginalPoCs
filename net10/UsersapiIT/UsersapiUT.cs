using Microsoft.AspNetCore.Mvc.Testing;

using System.Text.Json;
using System.Text;

using Usersapi;
public class UsersIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_EndpointReturnsSuccessAndCorrectContentType()
    {
        var newUser = new User(){ Name = "John Doe", Id=1, Address="blr, sada siva nagar" };
        var jsonPayload = JsonSerializer.Serialize(newUser);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        // Act
        var response = await _client.PostAsync("/api/users/users", content);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
    }
}
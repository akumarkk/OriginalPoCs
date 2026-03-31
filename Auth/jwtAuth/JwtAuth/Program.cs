using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

Console.WriteLine("=== JWT Validator ===");
Console.Write("Paste your token here: ");

// 1. Read the token and trim whitespace/newlines
string? tokenToValidate = Console.ReadLine()?.Trim();

if (string.IsNullOrWhiteSpace(tokenToValidate))
{
    Console.WriteLine("❌ No token provided. Exiting.");
    return;
}

// 1. Setup the Configuration Manager to fetch the Public Keys
var authority = Environment.GetEnvironmentVariable("AUTHORITY_URL") ?? "";
var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
    $"{authority}/.well-known/openid-configuration",
    new OpenIdConnectConfigurationRetriever());

// Fetch the current signing keys from the server
var config = await configurationManager.GetConfigurationAsync();
var publicKeys = config.SigningKeys;

// 2. Define Validation Parameters (Same as AddJwtBearer options)
var validationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = authority,
    
    ValidateAudience = false, // Your token didn't have an 'aud' claim
    
    ValidateLifetime = true,
    IssuerSigningKeys = publicKeys, // Use the keys we just downloaded
    
    // Optional: match the exact keys from your token
    NameClaimType = "client_id", 
    RoleClaimType = "roles"
};

// 3. The Validation Logic
var tokenHandler = new JwtSecurityTokenHandler();
// var tokenToValidate = "eyJhbGciOiJSUzI1..."; // Your long token string

try
{
    // This performs the signature, issuer, and expiry checks
    var principal = tokenHandler.ValidateToken(tokenToValidate, validationParameters, out SecurityToken validatedToken);

    Console.WriteLine("✅ Token is Valid!");
    Console.WriteLine($"Authenticated Client: {principal.Identity?.Name}");

    // Print all claims
    foreach (var claim in principal.Claims)
    {
        Console.WriteLine($"{claim.Type}: {claim.Value}");
    }
}
catch (SecurityTokenExpiredException)
{
    Console.WriteLine("❌ Token has expired.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Token validation failed: {ex.Message}");
}
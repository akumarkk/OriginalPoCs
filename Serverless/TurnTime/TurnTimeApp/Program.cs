using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using Serilog;
using Serilog.Settings.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using  Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
ValidateDynatraceEnvironmentVariables();

// Get the project root directory (go up from bin/output to the project root)
var logDirectory = Path.GetFullPath(Path.Combine(
    AppContext.BaseDirectory,
    "..", // out of 'output'
    ".." // out of 'bin'
));

// Set environment variable for Serilog to use
Environment.SetEnvironmentVariable("LOG_DIRECTORY", logDirectory);

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

static void ValidateDynatraceEnvironmentVariables()
{
    var requiredVars = new[]
    {
        "DT_TENANT",
        "DT_CLUSTER_ID",
        "DT_CONNECTION_BASE_URL",
        "DT_CONNECTION_AUTH_TOKEN"
    };

    var missingVars = requiredVars
        .Where(name => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name)))
        .ToList();

    if (missingVars.Any())
    {
        throw new InvalidOperationException($"Missing required Dynatrace environment variables: {string.Join(", ", missingVars)}");
    }
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); // Removes default console logger
    
    // Add Microsoft's built-in JSON console logger
    loggingBuilder.AddJsonConsole(options =>
    {
        options.IncludeScopes = true;
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
        options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
        {
            Indented = true // Set to false in production for better performance/smaller log sizes
        };
    });

    // causing duplicate metadata issue
    //loggingBuilder.AddSerilog(dispose: true);
});

builder.Services.AddSerilog(Log.Logger, dispose: true);

// builder.Services..AddOpenTelemetry()
//         .WithTracing(tracing => tracing
//             // .AddAzureFunctionsInstrumentation()
//             .AddDynatrace()
//             // ... if you need custom resources, set them after AddDynatrace
//         );

builder.ConfigureFunctionsWebApplication();

// builder.Services.AddOpenTelemetry()
//     .WithTracing(tracing =>
//     {
//         // If you have the Dynatrace OpenTelemetry NuGet package installed,
//         // ensure you have the correct 'using' statement for it at the top of the file.
//         tracing.AddDynatrace();
        
//         // ... if you need custom resources, set them after AddDynatrace
//     });

builder.Build().Run();

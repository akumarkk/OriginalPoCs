using Azure.Monitor.OpenTelemetry.Exporter;
using Dynatrace.OpenTelemetry;
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
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.ResourceDetectors.Azure;

// OpenTelemetry.Exporter removed to avoid missing assembly reference

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

// Register OpenTelemetry tracing and logging exporters (OTLP) in addition to Serilog.
// NOTE: Keeping both Serilog OpenTelemetry sink and the OpenTelemetry logging provider
// will result in duplicate OTLP exports for logs.
var dtTenantEnv = Environment.GetEnvironmentVariable("DT_TENANT") ?? string.Empty;
var dtTokenEnv = Environment.GetEnvironmentVariable("DT_CONNECTION_AUTH_TOKEN") ?? string.Empty;
var tracesEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT") ??
                      $"https://{dtTenantEnv}.live.dynatrace.com/api/v2/otlp/v1/traces";
var logsEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT") ??
                   $"https://{dtTenantEnv}.live.dynatrace.com/api/v2/otlp/v1/logs";
var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ??
                   $"https://{dtTenantEnv}.live.dynatrace.com/api/v2/otlp";
var otlpHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") ??
                  $"Authorization=Api-Token OTEL_TOKEN";

const string serviceName = "TurnTimeApp";
const string serviceVersion = "1.0.0";

// not required
// builder.Logging.AddOpenTelemetry(options =>
// {
//     options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion));
//     options.AddOtlpExporter(); // reads OTEL_EXPORTER_OTLP_* from environment
// });

// Use Dynatrace integration for OpenTelemetry tracing to avoid OTLP exporter
// assembly version conflicts. Traces will be sent via Dynatrace integration.
// builder.Services.AddOpenTelemetry()
//     .WithTracing(tracing => tracing
//         .SetResourceBuilder(ResourceBuilder.CreateDefault()
//             .AddService(serviceName, serviceVersion)
//             .AddAzureFunctions()
//             )
//         .AddDynatrace()
//     );

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "TurnTimeApp", serviceNamespace: "Serverless")
        // Instantiate the detector manually to completely bypass the missing extension method
        .AddDetector(new AppServiceResourceDetector()))
        .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation() // Essential: Tracks the incoming HTTP calls
        .AddOtlpExporter(options => 
        {
            options.Endpoint = new Uri(otlpEndpoint);
            options.Headers = otlpHeaders;
        }));;


// NOTE: We intentionally do not add the OpenTelemetry OTLP exporter here because
// mixing exporter packages caused a TypeLoadException in the runtime. Logs will
// continue to be exported by Serilog's configured sinks (including the OpenTelemetry
// Serilog sink if enabled in serilog.json).

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();

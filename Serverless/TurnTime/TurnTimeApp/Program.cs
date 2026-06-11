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
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

//  .MinimumLevel.Debug()
//     // Filters out framework/host noise so your files don't bloat
//     .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
//     .MinimumLevel.Override("Worker", LogEventLevel.Warning)
//     .MinimumLevel.Override("Host", LogEventLevel.Warning)
//     .Enrich.FromLogContext()
//     .WriteTo.Console()
//     .WriteTo.File(
//         path: "logs/function-log-.txt",
//         rollingInterval: RollingInterval.Day,
//         retainedFileCountLimit: 5
//     )


// var logPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "Logs", "function-logs-.txt"));
// Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

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

builder.ConfigureFunctionsWebApplication();

// builder.Services.AddOpenTelemetry()
//     .UseFunctionsWorkerDefaults()
//     .UseAzureMonitorExporter();

builder.Build().Run();

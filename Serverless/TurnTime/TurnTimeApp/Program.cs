using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var builder = FunctionsApplication.CreateBuilder(args);




Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    // Filters out framework/host noise so your files don't bloat
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Worker", LogEventLevel.Warning)
    .MinimumLevel.Override("Host", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/function-log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 5
    )
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); // Removes default console logger
    loggingBuilder.AddSerilog(dispose: true);
});

builder.ConfigureFunctionsWebApplication();

// builder.Services.AddOpenTelemetry()
//     .UseFunctionsWorkerDefaults()
//     .UseAzureMonitorExporter();

builder.Build().Run();

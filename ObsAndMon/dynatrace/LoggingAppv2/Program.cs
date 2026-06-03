using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

class Program
{
    static void Main(string[] args)
    {
        
        using var listener = new OtelDiagnosticListener();
    listener.EventWritten += (sender, eventArgs) =>
    {
        if (eventArgs.EventSource.Name.StartsWith("OpenTelemetry"))
        {
            Console.WriteLine($"[OTEL DIAGNOSTIC] {eventArgs.Message}");
        }
    };

        Console.WriteLine("Initializing OpenTelemetry Logger Factory...");

        // Define your service identity in Dynatrace
        var serviceName = "Sandbox-Console-Logger";
        var serviceVersion = "1.0.0";

        // Build the Logger Factory 
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                // Assign metadata so Dynatrace knows what service this log is from
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName, serviceVersion));

                // This automatically reads the OTEL_EXPORTER_OTLP_* variables from your environment
                options.AddOtlpExporter();
            });
        });

        // Create the standard .NET Logger instance
        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

        Console.WriteLine("Sending a test log to Dynatrace...");

        // Send a structured log line
        logger.LogInformation("Hello Dynatrace! This is a test log from my local .NET OpenTelemetry Console App at {Time}", DateTime.UtcNow);

        // Force OpenTelemetry to flush its memory buffer over the network before the console app closes
        Console.WriteLine("Flushing buffer... Please wait.");
        System.Threading.Thread.Sleep(3000); 

        Console.WriteLine("Done! Check your Dynatrace log viewer.");
    }
}

    public class OtelDiagnosticListener : System.Diagnostics.Tracing.EventListener
{
    protected override void OnEventSourceCreated(System.Diagnostics.Tracing.EventSource eventSource)
    {
        // Tell the listener to subscribe to all OpenTelemetry internal events
        if (eventSource.Name.StartsWith("OpenTelemetry"))
        {
            EnableEvents(eventSource, System.Diagnostics.Tracing.EventLevel.LogAlways);
        }
    }

    protected override void OnEventWritten(System.Diagnostics.Tracing.EventWrittenEventArgs eventData)
    {
        // Print the internal errors out to the console
        if (eventData.Message != null)
        {
            Console.WriteLine($"[OTEL DIAGNOSTIC] {string.Format(eventData.Message, eventData.Payload?.ToArray() ?? Array.Empty<object>())}");
        }
    }
}

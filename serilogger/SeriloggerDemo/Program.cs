using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Serilog;

namespace SeriloggerDemo;

class Program
{
    static void Main(string[] args)
    {
        // Initialize Serilog Logger with Context and Enrichers
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}.{MethodName}] [Env:{EnvironmentName}] [User:{EnvironmentUserName}] [Thread:{ThreadId}] [Process:{ProcessId}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            var rootLogger = Log.ForContext<Program>();
            rootLogger.ForContext("MethodName", nameof(Main))
                      .Information("Starting application...");
            
            var demo = new DemoWork();
            
            var logParam = new Dictionary<string, object>
            {
                { "Action", "Sample Action" },
                { "UserId", 42 },
                { "Status", "Active" }
            };
            demo.ProcessData(logParam);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.Information($"**Bye**");
            Log.CloseAndFlush();
        }
    }
}

public class DemoWork
{
    // Getting the class name into 'SourceContext' via strongly-typed context
    private readonly ILogger _logger = Log.ForContext<DemoWork>();

    // [CallerMemberName] injects the method name without manually passing it
    public void ProcessData(Dictionary<string, object> logParam, [CallerMemberName] string methodName = "")
    {
        // Add the methodName property to the current context
        var log = _logger.ForContext("MethodName", methodName);
        
        log.Information("Processing data: {@LogParam}", logParam);
        
        try
        {
            log.Debug("Simulating some work...");
            System.Threading.Thread.Sleep(500); // Simulate work to see thread info clearly
            throw new InvalidOperationException("Simulation exception to demonstrate error logging");
        }
        catch (Exception ex)
        {
            log.Error(ex, "An error occurred while processing data.");
        }
    }
}
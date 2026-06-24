###### ENV

Both DT_* and OTEL_* params required by  function app, for

```
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion, serviceNamespace: "Serverless")
        // Instantiate the detector manually to completely bypass the missing extension method
        .AddDetector(new AppServiceResourceDetector()))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation(); // Essential: Tracks the incoming HTTP calls

        // Use Dynatrace integration to avoid OTLP exporter assembly/version conflicts at runtime.
        // `ValidateDynatraceEnvironmentVariables()` ensures required DT env vars are present.
        tracing.AddDynatrace();
    });
```
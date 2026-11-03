# Logs in Application Insights but not in Dynatrace (Event Hub path)

## Summary

The Event Hub stream (`fo-dynatrace-appservice-logs`) carries only App Service **platform** diagnostic logs and metrics. Application Insights telemetry emitted by the app SDK/agent is **not** part of that stream, so it does not reach Dynatrace through it.

Evidence: 132 sampled Event Hub messages contained only `AppServiceHTTPLogs`, `AppServicePlatformLogs`, `AppServiceIPSecAuditLogs`, `AppServiceConsoleLogs`, and platform metrics. No Application Insights telemetry types were present.

## Available in Application Insights, missing in Dynatrace

| Application Insights data | In Dynatrace via Event Hub? | Notes |
|---|---|---|
| Requests (`requests` table) | No | Different from `AppServiceHTTPLogs`; includes operation IDs, duration, result code, success flag |
| Dependencies (SQL, HTTP, storage calls) | No | Outbound call timing and failures |
| Exceptions (with stack traces) | No | Console logs may show raw errors but not structured exceptions |
| Traces (`ILogger`/logging SDK output) | No | `AppServiceConsoleLogs` is only stdout/stderr, not the same records or severity metadata |
| Custom events and custom metrics | No | |
| Page views / browser telemetry | No | Only if the JS SDK is used |
| Availability tests | No | |
| Distributed tracing / operation correlation (`operation_Id`) | No | Platform logs have no end-to-end correlation ID |
| Application Map, Live Metrics, Profiler, Snapshot Debugger | No | Application Insights features, not exportable log data |

## Observed instance: ASP.NET Core port override warning

**Message** (seen in Application Insights `traces`, not found in Dynatrace):

```
Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://+:4200'.
```

- Source: `Microsoft.AspNetCore.Hosting.Diagnostics[15]`, severity `warn`, emitted at app startup.
- Meaning: `ASPNETCORE_URLS=http://+:4200` overrides `HTTP_PORTS=8080`. Removing one of the two settings clears the warning.

**Application Insights:** one `traces` record with `severityLevel` = Warning and the full message.

**Event Hub (`AppServiceConsoleLogs`):** the message is present, but as two separate records, because the console logger writes two lines and App Service sends one record per line. Example from the sample (2026-11-03 20:16:26 UTC):

| Time (UTC) | `resultDescription` | `level` |
|---|---|---|
| 20:16:26.906 | `warn: Microsoft.AspNetCore.Hosting.Diagnostics[15]` | Informational |
| 20:16:26.921 | `      Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://+:4200'.` | Informational |

**Why it is hard to find or missing in Dynatrace**
- The message text and the `warn` / logger name are in different records.
- `level` is always `Informational`, so the warning severity is lost and warning-based filters or alerts do not match.
- The text line is indented and has no operation ID or other context.
- If neither record appears in Dynatrace, check the forwarder and any ingestion filters, since the Event Hub does contain both.

**Fixes**
1. Dynatrace log-processing rule: merge lines that start with whitespace into the previous line, and derive severity from the `warn:` / `info:` / `fail:` prefix.
2. Change the app's console logging to single-line or JSON (`AddJsonConsole()`, or `AddSimpleConsole(o => o.SingleLine = true)`).
3. Send app logs to Dynatrace directly with OneAgent or OpenTelemetry.

The same applies to any multi-line .NET console log entry, such as exceptions with stack traces.

## Why Application Insights ingestion is still needed on top of Serilog and OneAgent

Assumed setup: the app logs through Serilog to Dynatrace, and Dynatrace OneAgent runs on the App Service. App Insights ingestion is justified only for the data those two do not provide:

| Reason | Detail |
|---|---|
| Different producers | Serilog sends only what app code logs through it. Application Insights also records platform-generated telemetry: requests, dependencies, exceptions, and operation correlation. |
| Logs outside Serilog | Entries written before Serilog is configured (host startup, such as the port override warning) or by other logging providers reach Application Insights but may never reach the Serilog sink. |
| Serilog filtering | Minimum-level overrides (for example `Microsoft: Warning`) drop framework logs that Application Insights still captures if its own filter is looser. |
| Azure-native consumers | Existing alerts, workbooks, Application Map, Live Metrics, smart detection, availability tests, and Azure support workflows read from Application Insights, not Dynatrace. |
| Other writers to the same resource | Other Azure components (Functions, API Management, other services) can write to the same Application Insights resource, so it holds cross-service traces that Dynatrace does not see. |
| Retention and compliance | Azure-side retention, access control, and audit through the Log Analytics workspace may be required independently of Dynatrace. |
| Migration safety | Keeping Application Insights during a Dynatrace rollout allows comparison and fallback until parity is confirmed. |

OneAgent covers tracing, metrics, and (depending on configuration) log capture, but it does not populate Application Insights tables or replace Azure-native tooling.

**When it is not required:** if the Azure-native consumers above are not used, and Serilog plus OneAgent already cover all needed logs, traces, and exceptions with correct severity, Application Insights ingestion is duplicate telemetry and cost. Confirm by comparing Application Insights `traces` and `exceptions` with the Dynatrace logs for the same time window before removing it.

## Not a gap

These are the reverse (Dynatrace only, not in Application Insights): HTTP access logs, platform/container lifecycle logs, IP restriction audit logs, and App Service platform metrics.

## How to close the gap if needed in Dynatrace

- Install Dynatrace OneAgent (App Service extension) or instrument with OpenTelemetry and export to Dynatrace.
- Or continuously export Application Insights data (workspace-based -> Log Analytics -> Event Hub via a data export rule) and forward to Dynatrace.

Both options add cost and duplicate telemetry if Application Insights is kept; confirm Dynatrace actually needs this data first.

## Related

See [AppServiceLogsToDynatrace.md](AppServiceLogsToDynatrace.md) for the opposite direction and the stream/skip decision.

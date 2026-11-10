# OTLP Endpoint & Dynatrace Auth Token: Pipeline vs Key Vault vs Code

## Status
**Decision:** Do **not** pass the OTLP endpoint or Dynatrace auth token as pipeline parameters.
Retrieve both from **Azure Key Vault** and expose them as environment variables:

- `OTEL_EXPORTER_OTLP_ENDPOINT` ← Key Vault secret `otelExporterOtlpEndpoint`
- `DT_CONNECTION_AUTH_TOKEN` ← Key Vault secret `dtConnectionAuthToken` (**required** for exporter auth)

This note compares all three approaches and records why Key Vault-backed configuration wins.

---

## Why not via pipeline parameters (`-otelExporterOtlpEndpoint "${{ parameters.OtelExporterOtlpEndpoint }}"`)

| Concern | Impact |
|---|---|
| **Secret leakage** | Pipeline parameters and command-line args appear in run history, debug logs, and process listings (`ps`). Tokens in command lines are visible to anyone with pipeline read access. |
| **Token rotation** | A token baked into YAML needs a pipeline edit + PR on every rotation. Key Vault centralizes rotation and can refresh without redeploying. |
| **Auditing & compliance** | Key Vault offers access policies, soft-delete, expiry tracking, audit logs. Pipeline variables have none of that. |
| **Environment sprawl** | Reused pipelines across dev/test/prod mean duplicated parameter sets, easy to misconfigure or copy a prod token into a lower env. |
| **Deploy-time vs runtime** | Endpoint and token are **runtime config**, not deployment config. The app should acquire them at startup, not have them stamped in at deploy time. |

> The endpoint URL itself is not a secret, but keeping endpoint + token together in Key Vault keeps configuration in one place and avoids a half-pipeline / half-Key Vault split.

---

## The three approaches compared

| | ✅ Option 1: Key Vault reference (host app settings) | ⚠️ Option 2: Pipeline injects from Key Vault | ❌ Option 3: Fetch token in Program.cs |
|---|---|---|---|
| Secret touches pipeline | Never | Yes (via KV task + secret variable) | Never |
| Code changes needed | None | None | Yes (SecretClient code) |
| Token rotation | Automatic (host refreshes KV references) | Requires redeploy | App restart + vault permission |
| App needs vault access | Host managed identity | Pipeline service connection | App managed identity |
| Startup risk | None | None | Extra latency/failure point; env-var race with exporter init |
| Logging exposure | None | Pipeline logs / command echo risk | Minimal |
| Verdict | **Preferred** | Acceptable fallback | Avoid |

---

## ✅ Option 1 (preferred): Key Vault reference in app settings

The app host (App Service / Functions / Container Apps) resolves Key Vault references via its **managed identity**. Nothing flows through the pipeline, and no code is needed, the OTLP exporter reads the environment variables natively.

```json
[
  {
    "name": "OTEL_EXPORTER_OTLP_ENDPOINT",
    "value": "@Microsoft.KeyVault(VaultName=myvault;SecretName=otelExporterOtlpEndpoint)"
  },
  {
    "name": "DT_CONNECTION_AUTH_TOKEN",
    "value": "@Microsoft.KeyVault(VaultName=myvault;SecretName=dtConnectionAuthToken)"
  }
]
```

**Pros**
- Secret never transits the pipeline or build agents.
- No Program.cs changes; exporter picks up env vars automatically.
- Automatic refresh on token rotation (App Service periodically re-resolves references).
- Full Key Vault auditing and access control.

**Cons**
- Requires the host to support Key Vault references (App Service family does; raw VMs need a startup task or sidecar).

---

## ⚠️ Option 2: pipeline fetches from Key Vault and injects

Use only when the runtime environment cannot resolve Key Vault references itself.

```yaml
- task: AzureKeyVault@2
  displayName: 'Fetch Dynatrace secrets'
  inputs:
    azureSubscription: 'my-service-connection'
    KeyVaultName: 'myvault'
    SecretsFilter: 'dtConnectionAuthToken,otelExporterOtlpEndpoint'

- task: AzureWebApp@1
  inputs:
    appSettings: >-
      -DT_CONNECTION_AUTH_TOKEN "$(dtConnectionAuthToken)"
      -OTEL_EXPORTER_OTLP_ENDPOINT "$(otelExporterOtlpEndpoint)"
```

**Pros**
- Works on any host, no managed identity needed on the app itself.
- Central rotation in Key Vault (though delivery still needs a redeploy).

**Cons**
- Secret transits pipeline memory and can leak into logs if tasks echo args.
- Rotation requires a pipeline run.
- Never use inline command-line args (visible in process listings); always map to app settings / environment variables.

---

## ❌ Option 3: fetch the token in Program.cs

Technically possible, worst of both worlds:

```csharp
// Program.cs — NOT recommended
var client = new SecretClient(
    new Uri("https://myvault.vault.azure.net/"),
    new DefaultAzureCredential());
var token = (await client.GetSecretAsync("dtConnectionAuthToken")).Value.Value;
Environment.SetEnvironmentVariable("DT_CONNECTION_AUTH_TOKEN", token); // hacky
```

**Problems**
- App needs its own vault permissions, an extra permission to manage and audit.
- Extra startup latency and a new failure mode (vault outage blocks app boot).
- Setting env vars from code can race with OpenTelemetry exporter initialization.
- Only justified for **non-Dynatrace** secrets the app consumes directly via `IConfiguration`.

---

## What Program.cs should actually contain

Nothing about the token. Only exporter wiring:

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
    .UseOtlpExporter();   // reads OTEL_EXPORTER_OTLP_ENDPOINT + DT_CONNECTION_AUTH_TOKEN from env
```

**Rule of thumb:** endpoint and token = environment variables sourced from Key Vault. Program.cs = exporter wiring only.

---

## Required Key Vault secrets (per environment)

| Secret name | Example value | Maps to |
|---|---|---|
| `otelExporterOtlpEndpoint` | `https://{env}.live.dynatrace.com/api/v2/otlp` | `OTEL_EXPORTER_OTLP_ENDPOINT` |
| `dtConnectionAuthToken` | Dynatrace API token with **Ingest OpenTelemetry traces / metrics / logs** scopes | `DT_CONNECTION_AUTH_TOKEN` |

---

## Checklist for new services

- [ ] OTLP endpoint stored in Key Vault, wired to `OTEL_EXPORTER_OTLP_ENDPOINT`
- [ ] Dynatrace token stored in Key Vault, wired to `DT_CONNECTION_AUTH_TOKEN`
- [ ] Token has minimum required scopes (OpenTelemetry ingest only)
- [ ] Token expiry date set and monitored in Key Vault
- [ ] No `dtConnectionAuthToken` / `otelExporterOtlpEndpoint` in pipeline parameters or command-line args
- [ ] Host managed identity has **Get** on the two secrets only (no broad vault access)

---

## Exceptions

If the runtime environment is air-gapped and cannot reach Key Vault at all, pass secrets as **pipeline secret variables mapped to environment variables** — never as inline command-line arguments.

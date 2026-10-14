# Clarify OpenTelemetry integration

## Architecture

The `Clarify.Observability` .NET 10 library is optional for generated applications. It exposes the shared `ActivitySource` and `Meter` named `Clarify.Application`, with reusable registration through `AddClarifyObservability(IConfiguration)`.

The current Blazor web host enables ASP.NET Core HTTP tracing and metrics and HttpClient instrumentation. Customer saves have a domain span, total/error counters and duration histograms. The library uses the established OpenTelemetry .NET SDK and OTLP exporter; it is **not a custom tracing backend**.

## Development configuration

The OTLP exporter is **disabled when no endpoint is configured**. The application continues to operate offline; no Collector is necessary.

Set `Clarify__Observability__ServiceName=Clarify.OrderEntry`.

For a local OpenTelemetry Collector with OTLP/HTTP configured, set:

```sh
Clarify__Observability__OtlpEndpoint=http://127.0.0.1:4318
```

The exporter protocol/port must match the Collector configuration; the default OTLP exporter settings may use gRPC on port 4317, so set compatible SDK options in a deployment before assuming port 4318 works. Do not expose telemetry receivers publicly without authentication/network restrictions.

## Privacy contract

- No passwords, tokens, HTTP bodies, form field contents, personal identifiers, SQL parameters or credential-bearing URLs in tags or logs.
- Exception **types**, stable technical entity names and durations may be recorded; exception messages can contain sensitive values and should not be exported by default.
- Treat application security audit logs separately from telemetry. Security logs require durable, access-controlled storage and retention policies.
- Per-app service names and sampling/export configuration should be generated from the project definition.
- Elsa already offers workflow instrumentation; integrate it through configuration rather than duplicating every workflow span.

## Tests and next increments

`tests/Clarify.M0.Tests/ObservabilityTests.cs` validates ActivitySource and Meter emission with in-process listeners. Additional required work: instrument order persistence, generated CRUD operations, Studio generation/builds, report renderer and permission denials; add Collector-backed integration tests using Testcontainers; verify no sensitive content is exported.

**Status:** Implementation and tests committed; .NET build/test has not been verified in the current authoring environment.

## Automatic generation — new default
The Clarify generator now emits `src/Clarify.Demo.Web/GeneratedObservability.cs` through `node tools/generate-observability.mjs` and the generated web host calls `AddGeneratedObservability(configuration)`. Developers do not have to add instrumentation registration manually.

`tests/automatic-observability.test.mjs` verifies the generated artifact is deterministic and checked into source control. Local verification scripts and GitHub Actions invoke the generator and reject source drift.

**Important boundary:** The present implementation targets the sample generated web host and instruments its HTTP requests and customer/order saves through the existing library. It is not yet a general multi-target generator for every Clarify application, nor does it automatically instrument every report, command, field-validation path or Elsa activity. Those remain tracked in issue #31.

**Defaults:** operational spans and counters are enabled; OTLP network export remains off unless an endpoint is configured. No application data values are intentionally recorded in Clarify's custom spans. Backend instrumentation and exporters require privacy and environment validation.

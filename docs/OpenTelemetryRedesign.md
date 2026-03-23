# OpenTelemetry Redesign

## Context

The current `SharpAbp.Abp.OpenTelemetry` implementation is usable but the abstraction boundary is still too weak:

- core options mix resource, signal, exporter selection, and low-level builder actions
- exporter semantics are partially string-driven
- `Collector` is easy to misunderstand as a standalone exporter type
- configuration-by-code is possible but not expressive enough for ABP-style module setup
- some API choices are coupled to older OpenTelemetry usage patterns

This document defines the next target architecture before the next implementation phase.

## Goals

- align with ABP vNext module and options patterns
- separate core observability abstractions from concrete exporters
- support both configuration-file setup and code-first module setup cleanly
- make configuration-file setup the primary and simplest onboarding path
- support OTLP direct export and OTLP via Collector without conflating protocol and deployment role
- support per-signal configuration for tracing, metrics, and logging
- minimize reliance on obsolete or confusing API patterns
- make the public API obvious for package consumers

## Non-Goals

- introducing a custom telemetry SDK on top of OpenTelemetry
- hiding all OpenTelemetry concepts behind SharpAbp-specific naming
- supporting every exporter in the core package
- redesigning unrelated observability documentation in the same step

## Design Principles

- core package owns signal orchestration, not exporter specifics
- exporters are modeled by actual exporter type, not by deployment topology
- Collector is an OTLP endpoint scenario, not a first-class protocol family
- configuration classes should be strongly typed and composable
- configuration file usage should be simpler than code-first usage for common scenarios
- code-first APIs should extend configuration, not compensate for a weak configuration model
- module consumers should be able to configure everything via `PreConfigure` / `Configure`
- defaults should be safe across target frameworks in this repository

## Package Boundaries

### Core Packages

- `SharpAbp.Abp.OpenTelemetry.Abstractions`
  - shared enums and stable cross-package contracts
  - exporter names only when truly needed across packages
- `SharpAbp.Abp.OpenTelemetry`
  - ABP module
  - resource configuration
  - tracing/metrics/logging signal configuration
  - code-first configuration DSL
  - exporter selection pipeline

### Exporter Packages

- `SharpAbp.Abp.OpenTelemetry.Exporter.Otlp`
- `SharpAbp.Abp.OpenTelemetry.Exporter.Console`
- `SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.AspNetCore`
- `SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.HttpListener`

The core package must not know OTLP or Prometheus-specific details.

## Conceptual Model

There are four different concerns and they must stay separate:

1. resource metadata
2. signal pipeline
3. exporter type
4. endpoint topology

### Resource Metadata

Examples:

- service name
- service namespace
- service version
- service instance id

### Signal Pipeline

Signals are:

- tracing
- metrics
- logging

Each signal can define:

- whether it is enabled
- instrumentation registrations
- OpenTelemetry builder customizations
- exporter selection

### Exporter Type

Examples:

- OTLP
- Console
- Prometheus

### Endpoint Topology

Examples for OTLP:

- direct to backend
- via OpenTelemetry Collector

This means:

- `Collector` is not a parallel exporter to `OTLP`
- `Collector` is one possible OTLP destination pattern

## Target Type Model

### Core Options

```csharp
public class AbpOpenTelemetryOptions
{
    public AbpOpenTelemetryResourceOptions Resource { get; set; }
    public AbpOpenTelemetryTracingOptions Tracing { get; set; }
    public AbpOpenTelemetryMetricsOptions Metrics { get; set; }
    public AbpOpenTelemetryLoggingOptions Logging { get; set; }

    public List<Action<IOpenTelemetryBuilder>> BuilderPreConfigures { get; }
    public List<Action<IOpenTelemetryBuilder>> BuilderPostConfigures { get; }
}
```

### Resource Options

```csharp
public class AbpOpenTelemetryResourceOptions
{
    public bool IsEnabled { get; set; }
    public string? ServiceName { get; set; }
    public string? ServiceNamespace { get; set; }
    public string? ServiceVersion { get; set; }
    public bool AutoGenerateServiceInstanceId { get; set; }
    public string? ServiceInstanceId { get; set; }
}
```

### Tracing Options

```csharp
public class AbpOpenTelemetryTracingOptions
{
    public bool IsEnabled { get; set; }
    public List<string> SourceNames { get; }
    public Action<TracerProviderBuilder>? SamplerConfigure { get; set; }
    public List<Action<TracerProviderBuilder>> InstrumentationConfigures { get; }
    public List<Action<TracerProviderBuilder>> BuilderConfigures { get; }
    public string? ExporterName { get; set; }
}
```

### Metrics Options

```csharp
public class AbpOpenTelemetryMetricsOptions
{
    public bool IsEnabled { get; set; }
    public List<string> MeterNames { get; }
    public Action<MeterProviderBuilder>? ExemplarFilterConfigure { get; set; }
    public List<Action<MeterProviderBuilder>> InstrumentationConfigures { get; }
    public List<Action<MeterProviderBuilder>> ViewConfigures { get; }
    public List<Action<MeterProviderBuilder>> BuilderConfigures { get; }
    public string? ExporterName { get; set; }
}
```

### Logging Options

```csharp
public class AbpOpenTelemetryLoggingOptions
{
    public bool IsEnabled { get; set; }
    public bool ClearProviders { get; set; }
    public bool IncludeFormattedMessage { get; set; }
    public bool IncludeScopes { get; set; }
    public bool ParseStateValues { get; set; }
    public string? ExporterName { get; set; }
    public List<Action<OpenTelemetryLoggerOptions>> BuilderConfigures { get; }
}
```

### Exporter Registry Contracts

The core package should internally work with separated registries:

```csharp
Dictionary<string, Action<TracerProviderBuilder>>
Dictionary<string, Action<MeterProviderBuilder>>
Dictionary<string, Action<OpenTelemetryLoggerOptions>>
```

This registry can remain internal implementation detail or internal-facing options state.

The public model should prefer typed signal options instead of exposing registry complexity directly.

## OTLP Package Design

### Public OTLP Options

```csharp
public class AbpOpenTelemetryOtlpOptions
{
    public string? Name { get; set; }
    public string? Endpoint { get; set; }
    public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.HttpProtobuf;
    public string? Headers { get; set; }
    public int TimeoutMilliseconds { get; set; } = 10000;

    public AbpOpenTelemetryOtlpSignalOptions Tracing { get; set; }
    public AbpOpenTelemetryOtlpSignalOptions Metrics { get; set; }
    public AbpOpenTelemetryOtlpSignalOptions Logging { get; set; }
}
```

### Per-Signal OTLP Overrides

```csharp
public class AbpOpenTelemetryOtlpSignalOptions
{
    public string? Endpoint { get; set; }
    public OtlpExportProtocol? Protocol { get; set; }
    public string? Headers { get; set; }
    public int? TimeoutMilliseconds { get; set; }
}
```

### Collector Support

Collector support should not be modeled as a separate exporter package.

Instead, OTLP package should support two user-facing setup styles:

1. generic OTLP configuration
2. OTLP-to-Collector recommended profile

Recommended shape:

```csharp
public class AbpOpenTelemetryOtlpEndpointProfile
{
    public string? Name { get; set; } // "Direct", "Collector", or custom
    public string? Endpoint { get; set; }
    public OtlpExportProtocol? Protocol { get; set; }
}
```

This profile can be optional. It exists only if it improves usability.

If it does not add real value, then the simpler rule is better:

- OTLP exporter always exports via OTLP
- if endpoint points to Collector, the app is using Collector mode

That simpler rule is preferred unless profiles significantly improve documentation and DX.

## Public Code-First API

The target API should feel like ABP module configuration rather than low-level OpenTelemetry plumbing.

### Recommended Core Setup

```csharp
PreConfigure<AbpOpenTelemetryOptions>(options =>
{
    options.Resource.ServiceName = "MyService";
    options.Resource.ServiceVersion = "1.0.0";

    options.Tracing.IsEnabled = true;
    options.Tracing.SourceNames.Add("MyService");

    options.Metrics.IsEnabled = true;
    options.Metrics.MeterNames.Add("MyService");

    options.Logging.IsEnabled = true;
    options.Logging.IncludeScopes = true;
});
```

### Recommended Fluent Helpers

```csharp
PreConfigure<AbpOpenTelemetryOptions>(options =>
{
    options
        .UseTracing(tracing =>
        {
            tracing.AddSource("MyService");
            tracing.UseExporter("Otlp");
        })
        .UseMetrics(metrics =>
        {
            metrics.AddMeter("MyService");
            metrics.UseExporter("PrometheusAspNetCore");
        })
        .UseLogging(logging =>
        {
            logging.UseExporter("Otlp");
            logging.IncludeScopes = true;
        });
});
```

### Recommended OTLP Setup

```csharp
PreConfigure<AbpOpenTelemetryOtlpOptions>(options =>
{
    options.Endpoint = "http://otel-collector:4318";
    options.Protocol = OtlpExportProtocol.HttpProtobuf;
    options.Headers = "api-key=demo";

    options.Tracing.Endpoint = "http://otel-collector:4318/v1/traces";
});
```

### Optional Convenience Extensions

```csharp
options.Tracing.UseOtlp();
options.Metrics.UsePrometheusAspNetCore();
options.Logging.UseOtlp();
```

These helpers should set exporter names only.

They should not hide the typed exporter options objects.

## Configuration File Model

Configuration file setup is the preferred default experience.

The common case should require only:

- enabling the needed signals
- selecting the exporter name for each signal
- filling one exporter block such as OTLP or Prometheus

Code configuration should only be necessary when:

- custom instrumentation must be added
- per-module runtime decisions are required
- builder delegates or advanced customization are needed

### Proposed `appsettings.json`

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyService",
      "ServiceNamespace": "SharpAbp.Sample",
      "ServiceVersion": "1.0.0",
      "AutoGenerateServiceInstanceId": true
    },
    "Tracing": {
      "IsEnabled": true,
      "ExporterName": "Otlp",
      "SourceNames": [ "MyService" ]
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "PrometheusAspNetCore",
      "MeterNames": [ "MyService" ]
    },
    "Logging": {
      "IsEnabled": true,
      "ExporterName": "Otlp",
      "IncludeScopes": true,
      "IncludeFormattedMessage": true,
      "ParseStateValues": true
    }
  },
  "OpenTelemetryExporters": {
    "Otlp": {
      "Endpoint": "http://otel-collector:4318",
      "Protocol": "HttpProtobuf",
      "Headers": "api-key=demo",
      "TimeoutMilliseconds": 10000,
      "Tracing": {
        "Endpoint": "http://otel-collector:4318/v1/traces"
      }
    },
    "PrometheusAspNetCore": {
      "ScrapeEndpointPath": "/metrics"
    }
  }
}
```

### Simpler Common Case

The most common user journey should be able to configure OTLP or Collector with a short, flat structure:

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyService"
    },
    "Tracing": {
      "IsEnabled": true,
      "ExporterName": "Otlp"
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "Otlp"
    },
    "Logging": {
      "IsEnabled": true,
      "ExporterName": "Otlp"
    }
  },
  "OpenTelemetryExporters": {
    "Otlp": {
      "Endpoint": "http://otel-collector:4318",
      "Protocol": "HttpProtobuf"
    }
  }
}
```

This should already be enough for:

- direct OTLP export to a backend
- export through Collector when the endpoint points to Collector

Users should not be forced to configure per-signal blocks unless they need overrides.

### Configuration Simplicity Rules

- avoid deeply nested exporter config for common scenarios
- keep one default exporter block per exporter family
- only introduce per-signal override sections when needed
- do not require separate `Collector` config unless it adds clear user value
- choose intuitive section names that match package names and docs
- preserve backward-compatible config binding or migration warnings where practical

## Module Lifecycle Rules

### Core Module

The core module should:

- bind core options from configuration in `PreConfigureServices`
- resolve exporter registrations during `ConfigureServices`
- build the OpenTelemetry pipelines during `PostConfigureServices`
- avoid legacy API patterns that are no longer necessary

### Exporter Modules

Each exporter module should:

- bind exporter-specific options in `PreConfigureServices`
- register exporter delegates into the core exporter registry in `ConfigureServices`
- avoid configuring unrelated signals

### AspNetCore-Specific Modules

AspNetCore-specific exporters such as Prometheus scraping endpoint should:

- register exporter delegates during service configuration
- perform middleware setup during `OnApplicationInitialization`
- expose explicit options controlling whether middleware is auto-enabled

## Naming Rules

- use `Otlp` for exporter type
- do not use `Collector` as an exporter name in the final public API unless it is only an optional profile alias
- use `ExporterName` per signal instead of `UseTracingExporter`, `UseMetricsExporter`, `UseLoggingExporter`
- prefer `IsEnabled` over `WithTracing` / `WithMetrics` / `WithLogging`

## Migration Strategy

### Phase 1

- finalize the new options model
- introduce nested options types
- keep backward-compatible mapping from old configuration if possible

### Phase 2

- refactor the core module to use new nested options
- keep internal exporter registry
- move signal-specific logic into typed options accessors

### Phase 3

- refactor OTLP exporter around one public OTLP model
- treat Collector as OTLP endpoint scenario in docs and examples

### Phase 4

- update Prometheus modules to align with the new signal/exporter naming
- add tests for exporter resolution and middleware behavior

### Phase 5

- rewrite `docs/Observability.md`
- publish recommended direct-backend and Collector examples

## Compatibility Notes

- repository targets include `netstandard` variants, so default OTLP transport should remain `HttpProtobuf`
- `gRPC` should remain opt-in rather than default
- `AddLegacySource` and other outdated patterns should not be part of the new design unless a real compatibility case exists

## First Implementation Slice

The first coding slice after design approval should be:

1. replace flat core options with nested resource/signal options
2. update core module to consume the nested model
3. keep exporter modules functional with minimal breakage
4. only then reshape OTLP public configuration

This minimizes churn while moving toward the cleaner long-term architecture.

## Decision Summary

- keep core package exporter-agnostic
- model tracing/metrics/logging as first-class typed options
- keep OTLP as one exporter family
- document Collector as an OTLP destination scenario
- provide both configuration-file setup and ABP-style code-first setup
- prefer explicit, strongly typed APIs over string-heavy configuration flows

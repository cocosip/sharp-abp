# Observability

OpenTelemetry integration for tracing, metrics, and logging in SharpAbp modules.

## OpenTelemetry

`SharpAbp.Abp.OpenTelemetry` provides the core ABP module and configuration model.

Exporter packages add concrete output targets such as OTLP, Console, and Prometheus.

## Installation

```bash
# Core packages
dotnet add package SharpAbp.Abp.OpenTelemetry
dotnet add package SharpAbp.Abp.OpenTelemetry.Abstractions

# Exporters
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Console
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.AspNetCore
dotnet add package SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus.HttpListener
```

## Configuration First

Configuration file setup is the recommended default path.

The core configuration is split into four sections:

- `OpenTelemetry:Resource`
- `OpenTelemetry:Tracing`
- `OpenTelemetry:Metrics`
- `OpenTelemetry:Logging`

For common scenarios, `Tracing:SourceNames` and `Metrics:MeterNames` can be omitted.
When they are empty, SharpAbp uses `OpenTelemetry:Resource:ServiceName` as the default source and meter name.

Exporter-specific settings are configured separately, for example:

- `OpenTelemetryExporters:Otlp`
- `OpenTelemetryExporters:PrometheusAspNetCore`

A complete sample file is available at [OpenTelemetry.appsettings.example.json](/D:/dotnet-code/sharp-abp/docs/OpenTelemetry.appsettings.example.json).

The OTLP sample in that file already uses OpenTelemetry Collector:

- `OpenTelemetryExporters:Otlp:Endpoint = http://otel-collector:4318`
- `Tracing/Metrics/Logging` can optionally override to Collector signal paths such as `/v1/traces`

### Minimal OTLP Example

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication",
      "ServiceNamespace": "SharpAbp.Sample",
      "ServiceVersion": "1.0.0",
      "AutoGenerateServiceInstanceId": true
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
      "ExporterName": "Otlp",
      "IncludeFormattedMessage": true,
      "IncludeScopes": true,
      "ParseStateValues": true
    }
  },
  "OpenTelemetryExporters": {
    "Otlp": {
      "Endpoint": "http://localhost:4318",
      "Protocol": "HttpProtobuf",
      "TimeoutMilliseconds": 10000
    }
  }
}
```

### OTLP and Collector

There is only one OTLP exporter in the module design.

If the OTLP endpoint points to an OpenTelemetry Collector, then the application is exporting through Collector.

Examples:

- direct OTLP backend: `http://vendor-endpoint:4318`
- OpenTelemetry Collector: `http://otel-collector:4318`

This means Collector is a deployment target for OTLP, not a separate exporter type.

### Collector Example

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication"
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
      "ExporterName": "Otlp",
      "IncludeFormattedMessage": true,
      "IncludeScopes": true,
      "ParseStateValues": true
    }
  },
  "OpenTelemetryExporters": {
    "Otlp": {
      "Endpoint": "http://otel-collector:4318",
      "Protocol": "HttpProtobuf",
      "TimeoutMilliseconds": 10000,
      "Tracing": {
        "Endpoint": "http://otel-collector:4318/v1/traces"
      },
      "Metrics": {
        "Endpoint": "http://otel-collector:4318/v1/metrics"
      },
      "Logging": {
        "Endpoint": "http://otel-collector:4318/v1/logs"
      }
    }
  }
}
```

Nothing special is required in code for Collector mode.

The current implementation already supports it because:

- OTLP exporter configuration binds `Endpoint`, `Protocol`, `Headers`, and per-signal overrides from `OpenTelemetryExporters:Otlp`
- the exporter module applies those values directly into `AddOtlpExporter(...)`
- if the endpoint is a Collector address, the application exports to Collector through the standard OTLP exporter

### Prometheus Example

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication"
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "PrometheusAspNetCore"
    }
  },
  "OpenTelemetryExporters": {
    "PrometheusAspNetCore": {
      "Name": "prometheus-aspnetcore",
      "ScrapeEndpointPath": "/metrics",
      "ScrapeResponseCacheDurationMilliseconds": 0,
      "UsePrometheusScrapingEndpoint": true
    }
  }
}
```

For ASP.NET Core applications, `PrometheusAspNetCore` is the usual choice.

`UsePrometheusScrapingEndpoint = true` means SharpAbp will register the default scraping endpoint middleware.

If you provide a custom `PrometheusScrapingEndpointConfigure` action in code, that custom action replaces the default endpoint registration path.

### Prometheus HttpListener Example

Use `PrometheusHttpListener` only for standalone listener scenarios where ASP.NET Core endpoint middleware is not the right fit.

```json
{
  "OpenTelemetry": {
    "Resource": {
      "IsEnabled": true,
      "ServiceName": "MyApplication"
    },
    "Metrics": {
      "IsEnabled": true,
      "ExporterName": "PrometheusHttpListener"
    }
  },
  "OpenTelemetryExporters": {
    "PrometheusHttpListener": {
      "Name": "prometheus-httplistener",
      "ScrapeEndpointPath": "/metrics",
      "UriPrefixes": [ "http://localhost:9464/" ]
    }
  }
}
```

`UriPrefixes` must contain at least one non-empty URI prefix.

## Module Setup

```csharp
[DependsOn(
    typeof(AbpOpenTelemetryModule),
    typeof(AbpOpenTelemetryExporterOtlpModule),
    typeof(AbpOpenTelemetryExporterPrometheusAspNetCoreModule)
)]
public class MyModule : AbpModule
{
}
```

For common scenarios, configuration file setup is enough.

## Code Configuration

Code configuration is useful when you need custom instrumentation or advanced builder behavior.

```csharp
public override void PreConfigureServices(ServiceConfigurationContext context)
{
    PreConfigure<AbpOpenTelemetryOptions>(options =>
    {
        options.Resource.ServiceName = "MyApplication";
        options.Resource.ServiceVersion = "1.0.0";

        options.Tracing.IsEnabled = true;
        options.Tracing.ExporterName = OpenTelemetryExporterNames.Otlp;

        options.Metrics.IsEnabled = true;
        options.Metrics.ExporterName = OpenTelemetryExporterNames.PrometheusAspNetCore;

        options.Logging.IsEnabled = true;
        options.Logging.ExporterName = OpenTelemetryExporterNames.Otlp;
        options.Logging.IncludeScopes = true;
    });
}
```

If you need custom `ActivitySource` or `Meter` names that differ from `Resource.ServiceName`, configure `Tracing.SourceNames` or `Metrics.MeterNames` explicitly.

## Exporter Configuration Notes

### OTLP

`OpenTelemetryExporters:Otlp` supports:

- `Name`
- `Endpoint`
- `Headers`
- `TimeoutMilliseconds`
- `Protocol`
- `Tracing`
- `Metrics`
- `Logging`

The nested `Tracing`, `Metrics`, and `Logging` sections are optional and are only needed when a signal must override the shared OTLP settings.

### Console

Console exporter can be used for tracing, metrics, or logging in development.

### Prometheus

- `PrometheusAspNetCore` is for ASP.NET Core applications and can expose `/metrics`
- `PrometheusHttpListener` is for standalone listener scenarios
- enable only the metrics exporter you actually want to use for the current process

## Best Practices

### Resource

Keep `ServiceName`, `ServiceNamespace`, and `ServiceVersion` stable and meaningful across environments.

### Signal Naming

- use clear `ActivitySource` names for tracing
- use stable `Meter` names for metrics
- prefer hierarchical names such as `MyCompany.Ordering`

### OTLP Protocol

For this repository's target frameworks, `HttpProtobuf` is the safest default.

Use `Grpc` only when your runtime and infrastructure requirements are confirmed.

### Configuration Simplicity

Prefer:

- one shared OTLP exporter block
- signal-level exporter names
- per-signal OTLP overrides only when required

Avoid duplicating exporter configuration across tracing, metrics, and logging unless the destinations really differ.

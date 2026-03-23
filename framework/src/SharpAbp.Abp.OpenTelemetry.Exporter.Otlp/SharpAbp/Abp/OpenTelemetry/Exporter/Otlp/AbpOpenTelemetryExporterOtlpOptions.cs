using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;
using System;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
{
    public class AbpOpenTelemetryExporterOtlpOptions
    {
        public string? Name { get; set; }

        public string? Endpoint { get; set; }

        public string? Headers { get; set; }

        public int TimeoutMilliseconds { get; set; } = 10000;

        public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.HttpProtobuf;

        public Action<OtlpExporterOptions>? ConfigureExporterOptions { get; set; }

        public AbpOpenTelemetryOtlpSignalOptions Tracing { get; set; }

        public AbpOpenTelemetryOtlpSignalOptions Metrics { get; set; }

        public AbpOpenTelemetryOtlpSignalOptions Logging { get; set; }

        public AbpOpenTelemetryExporterOtlpOptions()
        {
            Tracing = new AbpOpenTelemetryOtlpSignalOptions();
            Metrics = new AbpOpenTelemetryOtlpSignalOptions();
            Logging = new AbpOpenTelemetryOtlpSignalOptions();
        }

        public AbpOpenTelemetryExporterOtlpOptions PreConfigure(IConfiguration configuration)
        {
            var options = configuration
                .GetSection("OpenTelemetryExporters:Otlp")
                .Get<AbpOpenTelemetryExporterOtlpOptions>();

            if (options != null)
            {
                Name = options.Name;
                Endpoint = options.Endpoint;
                Headers = options.Headers;
                TimeoutMilliseconds = options.TimeoutMilliseconds;
                Protocol = options.Protocol;
                Tracing = options.Tracing ?? new AbpOpenTelemetryOtlpSignalOptions();
                Metrics = options.Metrics ?? new AbpOpenTelemetryOtlpSignalOptions();
                Logging = options.Logging ?? new AbpOpenTelemetryOtlpSignalOptions();
            }

            return this;
        }

        public AbpOpenTelemetryExporterOtlpOptions Configure(Action<OtlpExporterOptions> configureExporterOptions)
        {
            ConfigureExporterOptions = configureExporterOptions;
            return this;
        }

        public AbpOpenTelemetryExporterOtlpOptions ConfigureTracing(Action<AbpOpenTelemetryOtlpSignalOptions> configure)
        {
            configure(Tracing);
            return this;
        }

        public AbpOpenTelemetryExporterOtlpOptions ConfigureMetrics(Action<AbpOpenTelemetryOtlpSignalOptions> configure)
        {
            configure(Metrics);
            return this;
        }

        public AbpOpenTelemetryExporterOtlpOptions ConfigureLogging(Action<AbpOpenTelemetryOtlpSignalOptions> configure)
        {
            configure(Logging);
            return this;
        }

        internal void ApplyTo(OtlpExporterOptions exporterOptions, AbpOpenTelemetryOtlpSignalOptions? signalOptions)
        {
            if (!string.IsNullOrWhiteSpace(Endpoint))
            {
                exporterOptions.Endpoint = new Uri(Endpoint);
            }

            if (!string.IsNullOrWhiteSpace(Headers))
            {
                exporterOptions.Headers = Headers;
            }

            exporterOptions.TimeoutMilliseconds = TimeoutMilliseconds;
            exporterOptions.Protocol = Protocol;

            ConfigureExporterOptions?.Invoke(exporterOptions);
            signalOptions?.ApplyTo(exporterOptions);
        }
    }
}

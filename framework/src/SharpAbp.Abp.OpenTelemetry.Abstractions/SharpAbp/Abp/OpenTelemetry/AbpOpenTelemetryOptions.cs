using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryOptions
    {
        public AbpOpenTelemetryResourceOptions Resource { get; set; }

        public AbpOpenTelemetryTracingOptions Tracing { get; set; }

        public AbpOpenTelemetryMetricsOptions Metrics { get; set; }

        public AbpOpenTelemetryLoggingOptions Logging { get; set; }

        public Dictionary<string, Action<TracerProviderBuilder>> TracingExporters { get; set; }

        public Dictionary<string, Action<MeterProviderBuilder>> MetricsExporters { get; set; }

        public Dictionary<string, Action<OpenTelemetryLoggerOptions>> LoggingExporters { get; set; }

        public List<Action<IOpenTelemetryBuilder>> OpenTelemetryBuilderPreConfigures { get; set; }

        public List<Action<IOpenTelemetryBuilder>> OpenTelemetryBuilderPostConfigures { get; set; }

        public AbpOpenTelemetryOptions()
        {
            Resource = new AbpOpenTelemetryResourceOptions();
            Tracing = new AbpOpenTelemetryTracingOptions();
            Metrics = new AbpOpenTelemetryMetricsOptions();
            Logging = new AbpOpenTelemetryLoggingOptions();

            OpenTelemetryBuilderPreConfigures = [];
            OpenTelemetryBuilderPostConfigures = [];

            TracingExporters = [];
            MetricsExporters = [];
            LoggingExporters = [];
        }

        public AbpOpenTelemetryOptions PreConfigure(IConfiguration configuration)
        {
            ApplyNewConfiguration(configuration);
            return this;
        }

        private void ApplyNewConfiguration(IConfiguration configuration)
        {
            var resourceOptions = configuration.GetSection("OpenTelemetry:Resource").Get<AbpOpenTelemetryResourceOptions>();
            if (resourceOptions != null)
            {
                Resource.IsEnabled = resourceOptions.IsEnabled;
                Resource.ServiceName = resourceOptions.ServiceName;
                Resource.ServiceNamespace = resourceOptions.ServiceNamespace;
                Resource.ServiceVersion = resourceOptions.ServiceVersion;
                Resource.AutoGenerateServiceInstanceId = resourceOptions.AutoGenerateServiceInstanceId;
                Resource.ServiceInstanceId = resourceOptions.ServiceInstanceId;
            }

            var tracingOptions = configuration.GetSection("OpenTelemetry:Tracing").Get<AbpOpenTelemetryTracingOptions>();
            if (tracingOptions != null)
            {
                Tracing.IsEnabled = tracingOptions.IsEnabled;
                Tracing.SourceNames = tracingOptions.SourceNames ?? [];
                Tracing.ExporterName = tracingOptions.ExporterName;
            }

            var metricsOptions = configuration.GetSection("OpenTelemetry:Metrics").Get<AbpOpenTelemetryMetricsOptions>();
            if (metricsOptions != null)
            {
                Metrics.IsEnabled = metricsOptions.IsEnabled;
                Metrics.MeterNames = metricsOptions.MeterNames ?? [];
                Metrics.ExporterName = metricsOptions.ExporterName;
            }

            var loggingOptions = configuration.GetSection("OpenTelemetry:Logging").Get<AbpOpenTelemetryLoggingOptions>();
            if (loggingOptions != null)
            {
                Logging.IsEnabled = loggingOptions.IsEnabled;
                Logging.ExporterName = loggingOptions.ExporterName;
                Logging.ClearProviders = loggingOptions.ClearProviders;
                Logging.IncludeFormattedMessage = loggingOptions.IncludeFormattedMessage;
                Logging.IncludeScopes = loggingOptions.IncludeScopes;
                Logging.ParseStateValues = loggingOptions.ParseStateValues;
            }
        }

        public AbpOpenTelemetryOptions UseExporter(
            string exporterName,
            OpenTelemetrySignalKinds signalKinds = OpenTelemetrySignalKinds.All)
        {
            if (signalKinds.HasFlag(OpenTelemetrySignalKinds.Tracing))
            {
                Tracing.ExporterName = exporterName;
            }

            if (signalKinds.HasFlag(OpenTelemetrySignalKinds.Metrics))
            {
                Metrics.ExporterName = exporterName;
            }

            if (signalKinds.HasFlag(OpenTelemetrySignalKinds.Logging))
            {
                Logging.ExporterName = exporterName;
            }

            return this;
        }

        public AbpOpenTelemetryOptions EnableTracing(params string[] sourceNames)
        {
            Tracing.IsEnabled = true;
            foreach (var sourceName in sourceNames.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                AddSource(sourceName);
            }

            return this;
        }

        public AbpOpenTelemetryOptions EnableMetrics(params string[] meterNames)
        {
            Metrics.IsEnabled = true;
            foreach (var meterName in meterNames.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                AddMeter(meterName);
            }

            return this;
        }

        public AbpOpenTelemetryOptions EnableLogging(bool clearProviders = false)
        {
            Logging.IsEnabled = true;
            Logging.ClearProviders = clearProviders;
            return this;
        }

        public AbpOpenTelemetryOptions AddSource(string sourceName)
        {
            if (Tracing.SourceNames.All(x => !string.Equals(x, sourceName, StringComparison.OrdinalIgnoreCase)))
            {
                Tracing.SourceNames.Add(sourceName);
            }

            return this;
        }

        public AbpOpenTelemetryOptions AddMeter(string meterName)
        {
            if (Metrics.MeterNames.All(x => !string.Equals(x, meterName, StringComparison.OrdinalIgnoreCase)))
            {
                Metrics.MeterNames.Add(meterName);
            }

            return this;
        }

        public AbpOpenTelemetryOptions AddTracingInstrumentation(Action<TracerProviderBuilder> configure)
        {
            Tracing.InstrumentationConfigures.Add(configure);
            return this;
        }

        public AbpOpenTelemetryOptions AddMetricsInstrumentation(Action<MeterProviderBuilder> configure)
        {
            Metrics.InstrumentationConfigures.Add(configure);
            return this;
        }

        public AbpOpenTelemetryOptions ConfigureBuilder(Action<IOpenTelemetryBuilder> configure)
        {
            OpenTelemetryBuilderPreConfigures.Add(configure);
            return this;
        }

        public AbpOpenTelemetryOptions PostConfigureBuilder(Action<IOpenTelemetryBuilder> configure)
        {
            OpenTelemetryBuilderPostConfigures.Add(configure);
            return this;
        }
    }
}

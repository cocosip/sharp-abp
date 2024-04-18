using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryOptions
    {
        /// <summary>
        /// WithTracing
        /// </summary>
        public bool WithTracing { get; set; }

        /// <summary>
        /// Tracing names
        /// </summary>
        public List<string> SourceNames { get; set; }

        /// <summary>
        /// Tracing LegacySource operationName
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        ///  Tracing sampler
        /// </summary>
        public Action<TracerProviderBuilder> SamplerConfigure { get; set; }

        /// <summary>
        /// Tracing Instrumentation
        /// </summary>
        public List<Action<TracerProviderBuilder>> TracingInstrumentationConfigures { get; set; }

        /// <summary>
        /// Tracing exporter name
        /// </summary>
        public string UseTracingExporter { get; set; }

        /// <summary>
        /// Tracing exporter
        /// </summary>
        public Action<TracerProviderBuilder> TracingExporter { get; set; }

        /// <summary>
        /// Tracing Configures
        /// </summary>
        public List<Action<TracerProviderBuilder>> TracingConfigures { get; set; }

        /// <summary>
        /// WithMetrics
        /// </summary>
        public bool WithMetrics { get; set; }

        /// <summary>
        /// MeterNames
        /// </summary>
        public List<string> MeterNames { get; set; }

        /// <summary>
        /// Instrumentation
        /// </summary>
        public List<Action<MeterProviderBuilder>> MetricsInstrumentationConfigures { get; set; }

        /// <summary>
        /// ExemplarFilter
        /// </summary>
        public Action<MeterProviderBuilder> ExemplarFilterConfigure { get; set; }

        /// <summary>
        /// Metrics exporter name
        /// </summary>
        public string UseMetricsExporter { get; set; }

        /// <summary>
        /// Metrics views
        /// </summary>
        public List<Action<MeterProviderBuilder>> MetricsViewConfigures { get; set; }

        /// <summary>
        /// Metrics exporter
        /// </summary>
        public Action<MeterProviderBuilder> MetricsExporter { get; set; }

        /// <summary>
        ///  Metrics configures
        /// </summary>
        public List<Action<MeterProviderBuilder>> MetricsConfigures { get; set; }

        /// <summary>
        /// With logging
        /// </summary>
        public bool WithLogging { get; set; }

        /// <summary>
        /// Logging exporter
        /// </summary>
        public string UseLoggingExporter { get; set; }

        /// <summary>
        /// Logging exporter
        /// </summary>
        public Action<OpenTelemetryLoggerOptions> LoggingExporter { get; set; }

        public Dictionary<string, Action<TracerProviderBuilder>> TracingExporters { get; set; }
        public Dictionary<string, Action<MeterProviderBuilder>> MetricsExporters { get; set; }
        public Dictionary<string, Action<OpenTelemetryLoggerOptions>> LoggingExporters { get; set; }

        public List<Action<IOpenTelemetryBuilder>> OpenTelemetryBuilderPreConfigures { get; set; }
        public List<Action<IOpenTelemetryBuilder>> OpenTelemetryBuilderPostConfigures { get; set; }

        public AbpOpenTelemetryOptions()
        {
            SourceNames = [];
            TracingInstrumentationConfigures = [];
            TracingConfigures = [];

            MeterNames = [];
            MetricsInstrumentationConfigures = [];
            MetricsViewConfigures = [];
            MetricsConfigures = [];

            OpenTelemetryBuilderPreConfigures = [];
            OpenTelemetryBuilderPostConfigures = [];


            TracingExporters = [];
            MetricsExporters = [];
            LoggingExporters = [];
        }


        public AbpOpenTelemetryOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryOptions = configuration
                .GetSection("OpenTelemetryOptions")
                .Get<AbpOpenTelemetryOptions>();

            if (openTelemetryOptions != null)
            {
                WithTracing = openTelemetryOptions.WithTracing;
                SourceNames = openTelemetryOptions.SourceNames;
                OperationName = openTelemetryOptions.OperationName;
                UseTracingExporter = openTelemetryOptions.UseTracingExporter;
                WithMetrics = openTelemetryOptions.WithMetrics;
                MeterNames = openTelemetryOptions.MeterNames;
                UseMetricsExporter = openTelemetryOptions.UseMetricsExporter;
                WithLogging = openTelemetryOptions.WithLogging;
                UseLoggingExporter = openTelemetryOptions.UseLoggingExporter;
            }
            return this;
        }

    }
}

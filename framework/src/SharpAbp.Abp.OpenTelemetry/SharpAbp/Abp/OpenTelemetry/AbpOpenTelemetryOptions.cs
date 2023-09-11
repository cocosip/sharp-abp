using OpenTelemetry;
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

        public Dictionary<string, Action<TracerProviderBuilder>> TracingExporters { get; set; }
        public Dictionary<string, Action<MeterProviderBuilder>> MetricsExporters { get; set; }

        public List<Action<OpenTelemetryBuilder>> OpenTelemetryBuilderPreConfigures { get; set; }
        public List<Action<OpenTelemetryBuilder>> OpenTelemetryBuilderPostConfigures { get; set; }

        public AbpOpenTelemetryOptions()
        {
            SourceNames = new List<string>();
            TracingInstrumentationConfigures = new List<Action<TracerProviderBuilder>>();
            TracingConfigures = new List<Action<TracerProviderBuilder>>();

            MeterNames = new List<string>();
            MetricsInstrumentationConfigures = new List<Action<MeterProviderBuilder>>();
            MetricsViewConfigures = new List<Action<MeterProviderBuilder>>();
            MetricsConfigures = new List<Action<MeterProviderBuilder>>();

            OpenTelemetryBuilderPreConfigures = new List<Action<OpenTelemetryBuilder>>();
            OpenTelemetryBuilderPostConfigures = new List<Action<OpenTelemetryBuilder>>();

            TracingExporters = new Dictionary<string, Action<TracerProviderBuilder>>();
            MetricsExporters = new Dictionary<string, Action<MeterProviderBuilder>>();
        }
    }
}

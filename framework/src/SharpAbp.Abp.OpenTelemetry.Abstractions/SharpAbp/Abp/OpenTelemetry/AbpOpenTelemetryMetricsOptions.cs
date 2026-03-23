using OpenTelemetry.Metrics;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryMetricsOptions
    {
        public bool IsEnabled { get; set; }

        public List<string> MeterNames { get; set; }

        public string? ExporterName { get; set; }

        public Action<MeterProviderBuilder>? ExemplarFilterConfigure { get; set; }

        public Action<MeterProviderBuilder>? ExporterConfigure { get; set; }

        public List<Action<MeterProviderBuilder>> InstrumentationConfigures { get; set; }

        public List<Action<MeterProviderBuilder>> ViewConfigures { get; set; }

        public List<Action<MeterProviderBuilder>> BuilderConfigures { get; set; }

        public AbpOpenTelemetryMetricsOptions()
        {
            MeterNames = [];
            InstrumentationConfigures = [];
            ViewConfigures = [];
            BuilderConfigures = [];
        }
    }
}

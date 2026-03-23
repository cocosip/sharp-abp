using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryTracingOptions
    {
        public bool IsEnabled { get; set; }

        public List<string> SourceNames { get; set; }

        public string? ExporterName { get; set; }

        public Action<TracerProviderBuilder>? SamplerConfigure { get; set; }

        public Action<TracerProviderBuilder>? ExporterConfigure { get; set; }

        public List<Action<TracerProviderBuilder>> InstrumentationConfigures { get; set; }

        public List<Action<TracerProviderBuilder>> BuilderConfigures { get; set; }

        public AbpOpenTelemetryTracingOptions()
        {
            SourceNames = [];
            InstrumentationConfigures = [];
            BuilderConfigures = [];
        }
    }
}

using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryLoggingOptions
    {
        public bool IsEnabled { get; set; }

        public string? ExporterName { get; set; }

        public bool ClearProviders { get; set; }

        public bool IncludeFormattedMessage { get; set; }

        public bool IncludeScopes { get; set; }

        public bool ParseStateValues { get; set; }

        public Action<OpenTelemetryLoggerOptions>? ExporterConfigure { get; set; }

        public List<Action<OpenTelemetryLoggerOptions>> BuilderConfigures { get; set; }

        public AbpOpenTelemetryLoggingOptions()
        {
            BuilderConfigures = [];
        }
    }
}

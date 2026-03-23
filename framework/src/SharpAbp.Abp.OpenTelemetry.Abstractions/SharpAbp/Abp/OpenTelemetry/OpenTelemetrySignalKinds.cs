using System;

namespace SharpAbp.Abp.OpenTelemetry
{
    [Flags]
    public enum OpenTelemetrySignalKinds
    {
        None = 0,
        Tracing = 1,
        Metrics = 2,
        Logging = 4,
        All = Tracing | Metrics | Logging
    }
}

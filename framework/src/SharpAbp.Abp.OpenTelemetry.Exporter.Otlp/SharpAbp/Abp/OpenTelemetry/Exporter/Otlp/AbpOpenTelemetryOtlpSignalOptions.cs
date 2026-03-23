using OpenTelemetry.Exporter;
using System;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
{
    public class AbpOpenTelemetryOtlpSignalOptions
    {
        public string? Endpoint { get; set; }

        public string? Headers { get; set; }

        public int? TimeoutMilliseconds { get; set; }

        public OtlpExportProtocol? Protocol { get; set; }

        public Action<OtlpExporterOptions>? ConfigureExporterOptions { get; set; }

        internal void ApplyTo(OtlpExporterOptions exporterOptions)
        {
            if (!string.IsNullOrWhiteSpace(Endpoint))
            {
                exporterOptions.Endpoint = new Uri(Endpoint);
            }

            if (!string.IsNullOrWhiteSpace(Headers))
            {
                exporterOptions.Headers = Headers;
            }

            if (TimeoutMilliseconds.HasValue)
            {
                exporterOptions.TimeoutMilliseconds = TimeoutMilliseconds.Value;
            }

            if (Protocol.HasValue)
            {
                exporterOptions.Protocol = Protocol.Value;
            }

            ConfigureExporterOptions?.Invoke(exporterOptions);
        }
    }
}

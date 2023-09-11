using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
{
    public class AbpOpenTelemetryExporterOtlpOptions
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string Headers { get; set; }
        public int TimeoutMilliseconds { get; set; } = 10000;
        public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.Grpc;


        public AbpOpenTelemetryExporterOtlpOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryExporterOtlpOptions = configuration
                .GetSection("OpenTelemetryOptions:Exporters:Otlp")
                .Get<AbpOpenTelemetryExporterOtlpOptions>();

            if (openTelemetryExporterOtlpOptions != null)
            {
                Name = openTelemetryExporterOtlpOptions.Name;
                Endpoint = openTelemetryExporterOtlpOptions.Endpoint;
                Headers = openTelemetryExporterOtlpOptions.Headers;
                TimeoutMilliseconds = openTelemetryExporterOtlpOptions.TimeoutMilliseconds;
                Protocol = openTelemetryExporterOtlpOptions.Protocol;
            }
            return this;
        }
    }
}

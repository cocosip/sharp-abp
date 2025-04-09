using Microsoft.Extensions.Configuration;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Zipkin
{
    public class AbpOpenTelemetryExporterZipkinOptions
    {
        public string? Name { get; set; }
        public string? Endpoint { get; set; }
        public bool UseShortTraceIds { get; set; }
        public int? MaxPayloadSizeInBytes { get; set; }


        public AbpOpenTelemetryExporterZipkinOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryExporterZipkinOptions = configuration
                .GetSection("OpenTelemetryOptions:Exporters:Zipkin")
                .Get<AbpOpenTelemetryExporterZipkinOptions>();

            if (openTelemetryExporterZipkinOptions != null)
            {
                Name = openTelemetryExporterZipkinOptions.Name;
                Endpoint = openTelemetryExporterZipkinOptions.Endpoint;
                UseShortTraceIds = openTelemetryExporterZipkinOptions.UseShortTraceIds;
                MaxPayloadSizeInBytes = openTelemetryExporterZipkinOptions.MaxPayloadSizeInBytes;
            }
            return this;
        }
    }
}

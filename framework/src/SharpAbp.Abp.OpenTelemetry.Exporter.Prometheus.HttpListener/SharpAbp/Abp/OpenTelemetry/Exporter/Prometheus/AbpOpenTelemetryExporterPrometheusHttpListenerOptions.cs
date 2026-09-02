using Microsoft.Extensions.Configuration;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    public class AbpOpenTelemetryExporterPrometheusHttpListenerOptions
    {
        public string? Name { get; set; }

        public string? ScrapeEndpointPath { get; set; } = "/metrics";

        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 9464;

        public AbpOpenTelemetryExporterPrometheusHttpListenerOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryExporterPrometheusHttpListenerOptions = configuration
                .GetSection("OpenTelemetryExporters:PrometheusHttpListener")
                .Get<AbpOpenTelemetryExporterPrometheusHttpListenerOptions>();

            if (openTelemetryExporterPrometheusHttpListenerOptions != null)
            {
                Name = openTelemetryExporterPrometheusHttpListenerOptions.Name;
                ScrapeEndpointPath = openTelemetryExporterPrometheusHttpListenerOptions.ScrapeEndpointPath;
                Host = openTelemetryExporterPrometheusHttpListenerOptions.Host;
                Port = openTelemetryExporterPrometheusHttpListenerOptions.Port;
            }

            return this;
        }
    }
}

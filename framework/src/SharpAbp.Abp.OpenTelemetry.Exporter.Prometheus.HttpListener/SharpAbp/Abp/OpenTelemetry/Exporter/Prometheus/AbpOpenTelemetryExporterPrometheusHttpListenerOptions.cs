using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    public class AbpOpenTelemetryExporterPrometheusHttpListenerOptions
    {
        public string Name { get; set; }

        public string ScrapeEndpointPath { get; set; } = "/metrics";

        public List<string> UriPrefixes { get; set; }

        public AbpOpenTelemetryExporterPrometheusHttpListenerOptions()
        {
            UriPrefixes = new List<string>();
        }

        public AbpOpenTelemetryExporterPrometheusHttpListenerOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryExporterPrometheusHttpListenerOptions = configuration
                .GetSection("OpenTelemetryOptions:Exporters:PrometheusHttpListener")
                .Get<AbpOpenTelemetryExporterPrometheusHttpListenerOptions>();

            if (openTelemetryExporterPrometheusHttpListenerOptions != null)
            {
                Name = openTelemetryExporterPrometheusHttpListenerOptions.Name;
                ScrapeEndpointPath = openTelemetryExporterPrometheusHttpListenerOptions.ScrapeEndpointPath;
                UriPrefixes = openTelemetryExporterPrometheusHttpListenerOptions.UriPrefixes;
            }

            return this;
        }
    }
}

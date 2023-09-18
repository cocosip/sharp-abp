using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    public class AbpOpenTelemetryExporterPrometheusAspNetCoreOptions
    {
        public string Name { get; set; }

        public string ScrapeEndpointPath { get; set; } = "/metrics";

        public int ScrapeResponseCacheDurationMilliseconds { get; set; }

        public Action<IApplicationBuilder> PrometheusScrapingEndpointConfigure { get; set; }

        public AbpOpenTelemetryExporterPrometheusAspNetCoreOptions PreConfigure(IConfiguration configuration)
        {
            var openTelemetryExporterPrometheusAspNetCoreOptions = configuration
                .GetSection("OpenTelemetryOptions:Exporters:PrometheusAspNetCore")
                .Get<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>();

            if (openTelemetryExporterPrometheusAspNetCoreOptions != null)
            {
                Name = openTelemetryExporterPrometheusAspNetCoreOptions.Name;
                ScrapeEndpointPath = openTelemetryExporterPrometheusAspNetCoreOptions.ScrapeEndpointPath;
                ScrapeResponseCacheDurationMilliseconds = openTelemetryExporterPrometheusAspNetCoreOptions.ScrapeResponseCacheDurationMilliseconds;
            }
            return this;
        }
    }
}

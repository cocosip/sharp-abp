using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    public class AbpOpenTelemetryExporterPrometheusAspNetCoreOptions
    {
        public string? Name { get; set; }

        public string ScrapeEndpointPath { get; set; } = "/metrics";

        public int ScrapeResponseCacheDurationMilliseconds { get; set; }

        public bool UsePrometheusScrapingEndpoint { get; set; } = true;

        public Action<IApplicationBuilder>? PrometheusScrapingEndpointConfigure { get; set; }

        public AbpOpenTelemetryExporterPrometheusAspNetCoreOptions PreConfigure(IConfiguration configuration)
        {
            var options = configuration
                .GetSection("OpenTelemetryExporters:PrometheusAspNetCore")
                .Get<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>();

            if (options != null)
            {
                Name = options.Name;
                ScrapeEndpointPath = options.ScrapeEndpointPath;
                ScrapeResponseCacheDurationMilliseconds = options.ScrapeResponseCacheDurationMilliseconds;
                UsePrometheusScrapingEndpoint = options.UsePrometheusScrapingEndpoint;
            }

            return this;
        }
    }
}

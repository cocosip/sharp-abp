using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    public class AbpOpenTelemetryExporterPrometheusAspNetCoreOptionsTests
    {
        [Fact]
        public void PreConfigure_ShouldBind_PrometheusAspNetCoreSettings()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetryExporters:PrometheusAspNetCore:Name"] = "prometheus-aspnetcore",
                ["OpenTelemetryExporters:PrometheusAspNetCore:ScrapeEndpointPath"] = "/internal-metrics",
                ["OpenTelemetryExporters:PrometheusAspNetCore:ScrapeResponseCacheDurationMilliseconds"] = "250",
                ["OpenTelemetryExporters:PrometheusAspNetCore:UsePrometheusScrapingEndpoint"] = "false"
            });

            var options = new AbpOpenTelemetryExporterPrometheusAspNetCoreOptions().PreConfigure(configuration);

            Assert.Equal("prometheus-aspnetcore", options.Name);
            Assert.Equal("/internal-metrics", options.ScrapeEndpointPath);
            Assert.Equal(250, options.ScrapeResponseCacheDurationMilliseconds);
            Assert.False(options.UsePrometheusScrapingEndpoint);
        }

        private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}

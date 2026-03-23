using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    public class AbpOpenTelemetryExporterPrometheusHttpListenerOptionsTests
    {
        [Fact]
        public void PreConfigure_ShouldBind_UriPrefixes()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetryExporters:PrometheusHttpListener:Name"] = "prometheus-httplistener",
                ["OpenTelemetryExporters:PrometheusHttpListener:ScrapeEndpointPath"] = "/metrics",
                ["OpenTelemetryExporters:PrometheusHttpListener:UriPrefixes:0"] = "http://localhost:9464/"
            });

            var options = new AbpOpenTelemetryExporterPrometheusHttpListenerOptions().PreConfigure(configuration);

            Assert.Equal("prometheus-httplistener", options.Name);
            Assert.Equal("/metrics", options.ScrapeEndpointPath);
            Assert.Equal(["http://localhost:9464/"], options.UriPrefixes);
        }

        [Fact]
        public void PreConfigure_ShouldFallback_ToEmptyUriPrefixes()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetryExporters:PrometheusHttpListener:Name"] = "prometheus-httplistener"
            });

            var options = new AbpOpenTelemetryExporterPrometheusHttpListenerOptions().PreConfigure(configuration);

            Assert.Empty(options.UriPrefixes);
        }

        private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    public class AbpOpenTelemetryExporterPrometheusHttpListenerOptionsTests
    {
        [Fact]
        public void PreConfigure_ShouldBind_HostAndPort()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetryExporters:PrometheusHttpListener:Name"] = "prometheus-httplistener",
                ["OpenTelemetryExporters:PrometheusHttpListener:ScrapeEndpointPath"] = "/metrics",
                ["OpenTelemetryExporters:PrometheusHttpListener:Host"] = "metrics.example.test",
                ["OpenTelemetryExporters:PrometheusHttpListener:Port"] = "9465"
            });

            var options = new AbpOpenTelemetryExporterPrometheusHttpListenerOptions().PreConfigure(configuration);

            Assert.Equal("prometheus-httplistener", options.Name);
            Assert.Equal("/metrics", options.ScrapeEndpointPath);
            Assert.Equal("metrics.example.test", options.Host);
            Assert.Equal(9465, options.Port);
        }

        [Fact]
        public void PreConfigure_ShouldUseDefaultHostAndPort()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetryExporters:PrometheusHttpListener:Name"] = "prometheus-httplistener"
            });

            var options = new AbpOpenTelemetryExporterPrometheusHttpListenerOptions().PreConfigure(configuration);

            Assert.Equal("localhost", options.Host);
            Assert.Equal(9464, options.Port);
        }

        private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}

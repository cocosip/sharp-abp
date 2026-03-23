using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
{
    public class AbpOpenTelemetryExporterOtlpOptionsTests
    {
        [Fact]
        public void PreConfigure_ShouldBind_SharedAndSignalSpecific_OtlpSettings()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetryExporters:Otlp:Name"] = "otlp",
                ["OpenTelemetryExporters:Otlp:Endpoint"] = "http://localhost:4318",
                ["OpenTelemetryExporters:Otlp:Headers"] = "api-key=test",
                ["OpenTelemetryExporters:Otlp:TimeoutMilliseconds"] = "10000",
                ["OpenTelemetryExporters:Otlp:Protocol"] = "HttpProtobuf",
                ["OpenTelemetryExporters:Otlp:Tracing:Endpoint"] = "http://localhost:4318/v1/traces",
                ["OpenTelemetryExporters:Otlp:Logging:TimeoutMilliseconds"] = "5000"
            });

            var options = new AbpOpenTelemetryExporterOtlpOptions().PreConfigure(configuration);

            Assert.Equal("otlp", options.Name);
            Assert.Equal("http://localhost:4318", options.Endpoint);
            Assert.Equal("api-key=test", options.Headers);
            Assert.Equal(10000, options.TimeoutMilliseconds);
            Assert.Equal(global::OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf, options.Protocol);
            Assert.Equal("http://localhost:4318/v1/traces", options.Tracing.Endpoint);
            Assert.Equal(5000, options.Logging.TimeoutMilliseconds);
            Assert.Null(options.Metrics.Endpoint);
        }

        private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}

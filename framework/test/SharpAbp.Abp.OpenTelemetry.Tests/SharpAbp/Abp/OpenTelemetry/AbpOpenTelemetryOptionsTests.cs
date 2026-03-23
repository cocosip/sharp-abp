using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryOptionsTests
    {
        [Fact]
        public void PreConfigure_ShouldBind_NewSignalConfiguration()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetry:Tracing:IsEnabled"] = "true",
                ["OpenTelemetry:Tracing:ExporterName"] = OpenTelemetryExporterNames.Otlp,
                ["OpenTelemetry:Tracing:SourceNames:0"] = "orders-service",
                ["OpenTelemetry:Metrics:IsEnabled"] = "true",
                ["OpenTelemetry:Metrics:ExporterName"] = OpenTelemetryExporterNames.PrometheusAspNetCore,
                ["OpenTelemetry:Metrics:MeterNames:0"] = "orders-meter",
                ["OpenTelemetry:Logging:IsEnabled"] = "true",
                ["OpenTelemetry:Logging:ExporterName"] = OpenTelemetryExporterNames.Otlp,
                ["OpenTelemetry:Logging:IncludeFormattedMessage"] = "true",
                ["OpenTelemetry:Logging:IncludeScopes"] = "true",
                ["OpenTelemetry:Logging:ParseStateValues"] = "true"
            });

            var options = new AbpOpenTelemetryOptions().PreConfigure(configuration);

            Assert.True(options.Tracing.IsEnabled);
            Assert.Equal(OpenTelemetryExporterNames.Otlp, options.Tracing.ExporterName);
            Assert.Equal(["orders-service"], options.Tracing.SourceNames);
            Assert.True(options.Metrics.IsEnabled);
            Assert.Equal(OpenTelemetryExporterNames.PrometheusAspNetCore, options.Metrics.ExporterName);
            Assert.Equal(["orders-meter"], options.Metrics.MeterNames);
            Assert.True(options.Logging.IsEnabled);
            Assert.Equal(OpenTelemetryExporterNames.Otlp, options.Logging.ExporterName);
            Assert.True(options.Logging.IncludeFormattedMessage);
            Assert.True(options.Logging.IncludeScopes);
            Assert.True(options.Logging.ParseStateValues);
        }

        [Fact]
        public void PreConfigure_ShouldAllow_Omitting_SourceNames_And_MeterNames()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string?>
            {
                ["OpenTelemetry:Resource:IsEnabled"] = "true",
                ["OpenTelemetry:Resource:ServiceName"] = "orders-service",
                ["OpenTelemetry:Tracing:IsEnabled"] = "true",
                ["OpenTelemetry:Tracing:ExporterName"] = OpenTelemetryExporterNames.Otlp,
                ["OpenTelemetry:Metrics:IsEnabled"] = "true",
                ["OpenTelemetry:Metrics:ExporterName"] = OpenTelemetryExporterNames.Otlp
            });

            var options = new AbpOpenTelemetryOptions().PreConfigure(configuration);

            Assert.True(options.Tracing.IsEnabled);
            Assert.Empty(options.Tracing.SourceNames);
            Assert.True(options.Metrics.IsEnabled);
            Assert.Empty(options.Metrics.MeterNames);
        }

        [Fact]
        public void UseOtlpExporter_ShouldOnlyUpdate_SelectedSignals()
        {
            var options = new AbpOpenTelemetryOptions();

            options.UseOtlpExporter(OpenTelemetrySignalKinds.Tracing | OpenTelemetrySignalKinds.Logging);

            Assert.Equal(OpenTelemetryExporterNames.Otlp, options.Tracing.ExporterName);
            Assert.Null(options.Metrics.ExporterName);
            Assert.Equal(OpenTelemetryExporterNames.Otlp, options.Logging.ExporterName);
        }

        private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}

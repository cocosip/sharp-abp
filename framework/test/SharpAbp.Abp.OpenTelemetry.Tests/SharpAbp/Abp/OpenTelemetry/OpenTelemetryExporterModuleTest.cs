using System;
using global::OpenTelemetry;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.OpenTelemetry.Exporter.Console;
using SharpAbp.Abp.OpenTelemetry.Exporter.Otlp;
using SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus;
using Volo.Abp.Modularity;
using Volo.Abp;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry;

public class OpenTelemetryExporterModuleTest : AbpIntegratedTest<OpenTelemetryExporterTestModule>
{
    [Fact]
    public void Should_Register_Exporter_Capabilities_Without_Enabling_Signals()
    {
        var options = GetRequiredService<IOptions<AbpOpenTelemetryOptions>>().Value;
        Assert.Contains(OpenTelemetryExporterNames.Console, options.TracingExporters.Keys);
        Assert.Contains(OpenTelemetryExporterNames.Otlp, options.TracingExporters.Keys);
        Assert.Contains(OpenTelemetryExporterNames.Console, options.LoggingExporters.Keys);
        Assert.Contains(OpenTelemetryExporterNames.Otlp, options.LoggingExporters.Keys);
        Assert.Contains(OpenTelemetryExporterNames.PrometheusHttpListener, options.MetricsExporters.Keys);
        Assert.Null(ServiceProvider.GetService<TracerProvider>());
        Assert.Null(ServiceProvider.GetService<MeterProvider>());
    }

    [Theory]
    [InlineData("", 9464)]
    [InlineData("localhost", 0)]
    [InlineData("localhost", 65536)]
    public void Should_Reject_Invalid_Prometheus_Listener_Address_Before_Starting(string host, int port)
    {
        using var application = AbpApplicationFactory.Create<OpenTelemetryExporterTestModule>(creation =>
            creation.Services.PreConfigure<AbpOpenTelemetryExporterPrometheusHttpListenerOptions>(options =>
        {
            options.Host = host;
            options.Port = port;
        }));
        application.Initialize();
        var options = application.ServiceProvider.GetRequiredService<IOptions<AbpOpenTelemetryOptions>>().Value;

        Assert.Throws<InvalidOperationException>(() => options.MetricsExporters[OpenTelemetryExporterNames.PrometheusHttpListener](Sdk.CreateMeterProviderBuilder()));
    }

    [Theory]
    [InlineData(OpenTelemetryExporterNames.Console)]
    [InlineData(OpenTelemetryExporterNames.Otlp)]
    public void Should_Apply_Tracing_And_Metric_Exporter_Configuration(string exporterName)
    {
        var options = GetRequiredService<IOptions<AbpOpenTelemetryOptions>>().Value;
        var traces = Sdk.CreateTracerProviderBuilder();
        var metrics = Sdk.CreateMeterProviderBuilder();
        options.TracingExporters[exporterName](traces);
        options.MetricsExporters[exporterName](metrics);

        // Empty providers verify exporter construction without sending telemetry to a remote endpoint.
        using var tracer = traces.Build();
        using var meter = metrics.Build();
        Assert.NotNull(tracer);
        Assert.NotNull(meter);
    }
}

[DependsOn(typeof(AbpOpenTelemetryExporterConsoleModule), typeof(AbpOpenTelemetryExporterOtlpModule),
    typeof(AbpOpenTelemetryExporterPrometheusHttpListenerModule), typeof(Volo.Abp.AbpTestBaseModule))]
public class OpenTelemetryExporterTestModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpOpenTelemetryExporterConsoleOptions>(options =>
        {
            options.Name = "test-console";
            options.Targets = ConsoleExporterOutputTargets.Debug;
        });
    }
}

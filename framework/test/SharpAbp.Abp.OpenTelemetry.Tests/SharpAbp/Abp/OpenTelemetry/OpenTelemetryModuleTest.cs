using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using global::OpenTelemetry;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.OpenTelemetry;

public class OpenTelemetryModuleTest : AbpIntegratedTest<OpenTelemetrySignalTestModule>
{
    [Fact]
    public void Should_Export_Configured_Signals_Using_Case_Insensitive_Exporter_Selection()
    {
        var tracer = GetRequiredService<TracerProvider>();
        var metrics = GetRequiredService<MeterProvider>();
        var signals = GetRequiredService<TestTelemetrySignals>();
        using var source = new ActivitySource("test-source");
        using (var activity = source.StartActivity("test-operation"))
        {
            Assert.NotNull(activity);
        }
        using var meter = new Meter("test-meter");
        meter.CreateCounter<long>("test-counter").Add(3);
        GetRequiredService<ILogger<OpenTelemetryModuleTest>>().LogInformation("Test log {Value}", 42);
        metrics.ForceFlush();
        tracer.ForceFlush();

        Assert.Contains("test-operation", signals.Traces);
        Assert.Contains("test-counter", signals.Metrics);
        Assert.Contains("Test log 42", signals.Logs);
        Assert.Contains(tracer.GetResource().Attributes, pair => pair.Key == "service.name" && Equals(pair.Value, "test-service"));
        Assert.True(signals.BeforeBuilder);
        Assert.True(signals.AfterBuilder);
        Assert.True(signals.TracingInstrumentation);
        Assert.True(signals.MetricsInstrumentation);
        Assert.True(signals.MetricsView);
        Assert.True(signals.LogBuilder);
        Assert.Equal("tEsT", GetRequiredService<IOptions<AbpOpenTelemetryOptions>>().Value.Tracing.ExporterName);
    }
}

[DependsOn(typeof(AbpOpenTelemetryModule), typeof(Volo.Abp.AbpTestBaseModule))]
public class OpenTelemetrySignalTestModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var signals = new TestTelemetrySignals();
        context.Services.AddSingleton(signals);
        PreConfigure<AbpOpenTelemetryOptions>(options =>
        {
            options.Resource.IsEnabled = true;
            options.Resource.ServiceName = "test-service";
            options.Resource.ServiceNamespace = "tests";
            options.Resource.ServiceVersion = "1.0";
            options.Resource.ServiceInstanceId = "test-instance";
            options.EnableTracing("test-source").EnableMetrics("test-meter").ConfigureLogging(clearProviders: true).UseExporter("tEsT");
            options.TracingExporters["Test"] = builder => builder.AddProcessor(
                new SimpleActivityExportProcessor(new TestExporter<Activity>(activity => signals.Traces.Add(activity.DisplayName))));
            options.MetricsExporters["Test"] = builder => builder.AddReader(
                new PeriodicExportingMetricReader(new TestExporter<Metric>(metric => signals.Metrics.Add(metric.Name)), int.MaxValue));
            options.LoggingExporters["Test"] = builder => builder.AddProcessor(
                new SimpleLogRecordExportProcessor(new TestExporter<LogRecord>(record => signals.Logs.Add(record.FormattedMessage ?? ""))));
            options.ConfigureBuilder(_ => signals.BeforeBuilder = true);
            options.PostConfigureBuilder(_ => signals.AfterBuilder = true);
            options.AddTracingInstrumentation(_ => signals.TracingInstrumentation = true);
            options.AddMetricsInstrumentation(_ => signals.MetricsInstrumentation = true);
            options.Metrics.ViewConfigures.Add(_ => signals.MetricsView = true);
            options.Tracing.BuilderConfigures.Add(builder => builder.SetSampler(new AlwaysOnSampler()));
            options.Metrics.BuilderConfigures.Add(builder => builder.AddMeter("additional-meter"));
            options.Logging.BuilderConfigures.Add(_ => signals.LogBuilder = true);
        });
    }
}

public class TestTelemetrySignals
{
    public List<string> Traces { get; } = [];
    public List<string> Metrics { get; } = [];
    public List<string> Logs { get; } = [];
    public bool BeforeBuilder { get; set; }
    public bool AfterBuilder { get; set; }
    public bool TracingInstrumentation { get; set; }
    public bool MetricsInstrumentation { get; set; }
    public bool MetricsView { get; set; }
    public bool LogBuilder { get; set; }
}

internal sealed class TestExporter<T>(Action<T> capture) : BaseExporter<T> where T : class
{
    public override ExportResult Export(in Batch<T> batch)
    {
        foreach (var item in batch) { capture(item); }
        return ExportResult.Success;
    }
}

public class OpenTelemetryFallbackTest : AbpIntegratedTest<OpenTelemetryFallbackTestModule>
{
    [Fact]
    public void Should_Use_Legacy_Resource_And_Default_Source_When_Exporter_Is_Unknown()
    {
        var tracer = GetRequiredService<TracerProvider>();
        using var source = new ActivitySource("legacy-service");
        using var activity = source.StartActivity("fallback-operation");
        Assert.NotNull(activity);
        Assert.Contains(tracer.GetResource().Attributes, pair => pair.Key == "service.name" && Equals(pair.Value, "legacy-service"));
        Assert.Empty(GetRequiredService<TestTelemetrySignals>().Traces);
        Assert.NotNull(GetRequiredService<MeterProvider>());
    }
}

[DependsOn(typeof(AbpOpenTelemetryModule), typeof(Volo.Abp.AbpTestBaseModule))]
public class OpenTelemetryFallbackTestModule : OpenTelemetrySignalTestModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        base.PreConfigureServices(context);
        PreConfigure<AbpOpenTelemetryResourceOptions>(options =>
        {
            options.IsEnabled = true;
            options.ServiceName = "legacy-service";
            options.ServiceNamespace = "legacy";
            options.ServiceVersion = "2.0";
            options.ServiceInstanceId = "legacy-instance";
        });
        PreConfigure<AbpOpenTelemetryOptions>(options =>
        {
            options.Resource = new AbpOpenTelemetryResourceOptions();
            options.Tracing.SourceNames.Clear();
            options.Metrics.MeterNames.Clear();
            options.Tracing.ExporterName = "unknown";
            options.Metrics.ExporterName = null;
            options.Logging.ExporterName = " ";
        });
    }
}

public class OpenTelemetryDisabledTest : AbpIntegratedTest<OpenTelemetryDisabledTestModule>
{
    [Fact]
    public void Should_Not_Start_Providers_When_Resource_Is_Disabled()
    {
        Assert.Null(ServiceProvider.GetService<TracerProvider>());
        Assert.Null(ServiceProvider.GetService<MeterProvider>());
        Assert.False(GetRequiredService<TestTelemetrySignals>().BeforeBuilder);
        Assert.False(GetRequiredService<TestTelemetrySignals>().AfterBuilder);
    }
}

[DependsOn(typeof(AbpOpenTelemetryModule), typeof(Volo.Abp.AbpTestBaseModule))]
public class OpenTelemetryDisabledTestModule : OpenTelemetrySignalTestModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        base.PreConfigureServices(context);
        PreConfigure<AbpOpenTelemetryOptions>(options => options.Resource.IsEnabled = false);
    }
}

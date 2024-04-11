using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Console
{
    [DependsOn(
        typeof(AbpOpenTelemetryModule)
        )]
    public class AbpOpenTelemetryExporterConsoleModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryExporterConsoleOptions>(options =>
            {
                options.PreConfigure(configuration);
            });

            return Task.CompletedTask;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var openTelemetryExporterConsoleOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterConsoleOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                var action = new Action<ConsoleExporterOptions>(c =>
                {
                    c.Targets = openTelemetryExporterConsoleOptions.Targets;
                });

                options.TracingExporters.Add(OpenTelemetryExporterNames.Console, new Action<TracerProviderBuilder>(builder =>
                {
                    builder.AddConsoleExporter(openTelemetryExporterConsoleOptions.Name, action);
                }));

                options.MetricsExporters.Add(OpenTelemetryExporterNames.Console, new Action<MeterProviderBuilder>(builder =>
                {
                    builder.AddConsoleExporter(openTelemetryExporterConsoleOptions.Name, action);
                }));

                options.LoggingExporters.Add(OpenTelemetryExporterNames.Console, new Action<OpenTelemetryLoggerOptions>(builder =>
                {
                    builder.AddConsoleExporter(action);
                }));

            });

            return Task.CompletedTask;
        }

    }
}

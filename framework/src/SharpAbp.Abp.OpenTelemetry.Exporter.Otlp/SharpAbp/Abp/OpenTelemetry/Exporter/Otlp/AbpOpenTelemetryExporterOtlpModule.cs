using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
{
    [DependsOn(
        typeof(AbpOpenTelemetryModule)
        )]
    public class AbpOpenTelemetryExporterOtlpModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryExporterOtlpOptions>(options =>
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
            var openTelemetryExporterOtlpOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterOtlpOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                var action = new Action<OtlpExporterOptions>(c =>
                {
                    c.Endpoint = new Uri(openTelemetryExporterOtlpOptions.Endpoint);
                });

                options.TracingExporters.Add(OpenTelemetryExporterNames.Otlp, new Action<TracerProviderBuilder>(builder =>
                {
                    builder.AddOtlpExporter(openTelemetryExporterOtlpOptions.Name, action);
                }));

                options.MetricsExporters.Add(OpenTelemetryExporterNames.Otlp, new Action<MeterProviderBuilder>(builder =>
                {
                    builder.AddOtlpExporter(openTelemetryExporterOtlpOptions.Name, action);
                }));

                options.LoggingExporters.Add(OpenTelemetryExporterNames.Otlp, new Action<OpenTelemetryLoggerOptions>(builder =>
                {
                    builder.AddOtlpExporter((exporterOptions, processorOptions) =>
                    {
                        exporterOptions.Endpoint = new Uri(openTelemetryExporterOtlpOptions.Endpoint);
                    });
                }));
            });

            return Task.CompletedTask;
        }
    }
}

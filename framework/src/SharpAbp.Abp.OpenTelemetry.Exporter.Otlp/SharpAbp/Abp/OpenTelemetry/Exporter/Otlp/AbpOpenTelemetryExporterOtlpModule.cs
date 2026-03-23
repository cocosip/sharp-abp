using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Otlp
{
    [DependsOn(typeof(AbpOpenTelemetryModule))]
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
            var otlpOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterOtlpOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                RegisterExporter(options, OpenTelemetryExporterNames.Otlp, otlpOptions);
            });

            return Task.CompletedTask;
        }

        private static void RegisterExporter(
            AbpOpenTelemetryOptions options,
            string exporterName,
            AbpOpenTelemetryExporterOtlpOptions exporterOptions)
        {
            options.TracingExporters[exporterName] = builder =>
            {
                builder.AddOtlpExporter(exporterOptions.Name, otlpOptions =>
                {
                    exporterOptions.ApplyTo(otlpOptions, exporterOptions.Tracing);
                });
            };

            options.MetricsExporters[exporterName] = builder =>
            {
                builder.AddOtlpExporter(exporterOptions.Name, otlpOptions =>
                {
                    exporterOptions.ApplyTo(otlpOptions, exporterOptions.Metrics);
                });
            };

            options.LoggingExporters[exporterName] = builder =>
            {
                builder.AddOtlpExporter((otlpOptions, _) =>
                {
                    exporterOptions.ApplyTo(otlpOptions, exporterOptions.Logging);
                });
            };
        }
    }
}

using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    [DependsOn(typeof(AbpOpenTelemetryModule))]
    public class AbpOpenTelemetryExporterPrometheusHttpListenerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryExporterPrometheusHttpListenerOptions>(options =>
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
            var exporterOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterPrometheusHttpListenerOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                options.MetricsExporters[OpenTelemetryExporterNames.PrometheusHttpListener] = builder =>
                {
                    if (string.IsNullOrWhiteSpace(exporterOptions.Host))
                    {
                        throw new InvalidOperationException(
                            "OpenTelemetryExporters:PrometheusHttpListener:Host must be non-empty.");
                    }

                    if (exporterOptions.Port <= 0 || exporterOptions.Port > ushort.MaxValue)
                    {
                        throw new InvalidOperationException(
                            "OpenTelemetryExporters:PrometheusHttpListener:Port must be between 1 and 65535.");
                    }

                    builder.AddPrometheusHttpListener(exporterOptions.Name, prometheusOptions =>
                    {
                        prometheusOptions.ScrapeEndpointPath = exporterOptions.ScrapeEndpointPath;
                        prometheusOptions.Host = exporterOptions.Host;
                        prometheusOptions.Port = exporterOptions.Port;

                    });
                };
            });

            return Task.CompletedTask;
        }
    }
}

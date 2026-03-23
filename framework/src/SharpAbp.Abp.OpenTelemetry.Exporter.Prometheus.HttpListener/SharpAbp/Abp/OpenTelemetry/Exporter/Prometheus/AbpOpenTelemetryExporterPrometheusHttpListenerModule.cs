using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
                    if (exporterOptions.UriPrefixes == null || exporterOptions.UriPrefixes.Count == 0 ||
                        exporterOptions.UriPrefixes.Any(string.IsNullOrWhiteSpace))
                    {
                        throw new InvalidOperationException(
                            "OpenTelemetryExporters:PrometheusHttpListener:UriPrefixes must contain at least one non-empty URI prefix.");
                    }

                    builder.AddPrometheusHttpListener(exporterOptions.Name, prometheusOptions =>
                    {
                        prometheusOptions.ScrapeEndpointPath = exporterOptions.ScrapeEndpointPath;
                        prometheusOptions.UriPrefixes = exporterOptions.UriPrefixes;
                    });
                };
            });

            return Task.CompletedTask;
        }
    }
}

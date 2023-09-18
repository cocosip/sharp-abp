using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    [DependsOn(
        typeof(AbpOpenTelemetryModule)
        )]
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

            var openTelemetryExporterPrometheusHttpListenerOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterPrometheusHttpListenerOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                options.MetricsExporters.Add(OpenTelemetryExporterNames.PrometheusAspNetCore, new Action<MeterProviderBuilder>(builder =>
                {
                    builder.AddPrometheusHttpListener(openTelemetryExporterPrometheusHttpListenerOptions.Name, new Action<PrometheusHttpListenerOptions>(builder =>
                    {
                        builder.ScrapeEndpointPath = openTelemetryExporterPrometheusHttpListenerOptions.ScrapeEndpointPath;
                        builder.UriPrefixes = openTelemetryExporterPrometheusHttpListenerOptions.UriPrefixes;
                    }));
                }));
            });

            return base.ConfigureServicesAsync(context);
        }
    }
}

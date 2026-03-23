using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Prometheus
{
    [DependsOn(typeof(AbpOpenTelemetryModule), typeof(AbpAspNetCoreModule))]
    public class AbpOpenTelemetryExporterPrometheusAspNetCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>(options =>
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
            var exporterOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                options.MetricsExporters[OpenTelemetryExporterNames.PrometheusAspNetCore] = builder =>
                {
                    builder.AddPrometheusExporter(exporterOptions.Name, prometheusOptions =>
                    {
                        prometheusOptions.ScrapeEndpointPath = exporterOptions.ScrapeEndpointPath;
                        prometheusOptions.ScrapeResponseCacheDurationMilliseconds = exporterOptions.ScrapeResponseCacheDurationMilliseconds;
                    });
                };
            });

            return Task.CompletedTask;
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var preConfigures = context.Services.GetPreConfigureActions<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>();
            Configure<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>(options =>
            {
                foreach (var preConfigure in preConfigures)
                {
                    preConfigure?.Invoke(options);
                }
            });

            return Task.CompletedTask;
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
        }

        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var options = context.ServiceProvider.GetRequiredService<IOptions<AbpOpenTelemetryOptions>>().Value;
            var exporterOptions = context.ServiceProvider.GetRequiredService<IOptions<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>>().Value;
            var applicationBuilder = context.GetApplicationBuilder();

            if (options.Metrics.IsEnabled &&
                string.Equals(options.Metrics.ExporterName, OpenTelemetryExporterNames.PrometheusAspNetCore, StringComparison.OrdinalIgnoreCase))
            {
                if (exporterOptions.PrometheusScrapingEndpointConfigure != null)
                {
                    exporterOptions.PrometheusScrapingEndpointConfigure(applicationBuilder);
                }
                else if (exporterOptions.UsePrometheusScrapingEndpoint)
                {
                    applicationBuilder.UseOpenTelemetryPrometheusScrapingEndpoint();
                }
            }

            return Task.CompletedTask;
        }
    }
}

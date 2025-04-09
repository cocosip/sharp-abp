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
    [DependsOn(
        typeof(AbpOpenTelemetryModule),
        typeof(AbpAspNetCoreModule)
        )]
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
            var openTelemetryExporterPrometheusAspNetCoreOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                options.MetricsExporters.Add(OpenTelemetryExporterNames.PrometheusAspNetCore, new Action<MeterProviderBuilder>(builder =>
                {
                    builder.AddPrometheusExporter(openTelemetryExporterPrometheusAspNetCoreOptions.Name, new Action<PrometheusAspNetCoreOptions>(builder =>
                    {
                        builder.ScrapeEndpointPath = openTelemetryExporterPrometheusAspNetCoreOptions.ScrapeEndpointPath;
                        builder.ScrapeResponseCacheDurationMilliseconds = openTelemetryExporterPrometheusAspNetCoreOptions.ScrapeResponseCacheDurationMilliseconds;
                    }));
                }));
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
            var openTelemetryExporterPrometheusAspNetCoreOptions = context.ServiceProvider.GetRequiredService<IOptions<AbpOpenTelemetryExporterPrometheusAspNetCoreOptions>>().Value;

            var applicationBuilder = context.GetApplicationBuilder();

            if (options.UseMetricsExporter!.Equals(OpenTelemetryExporterNames.PrometheusAspNetCore, StringComparison.OrdinalIgnoreCase))
            {
                openTelemetryExporterPrometheusAspNetCoreOptions.PrometheusScrapingEndpointConfigure?.Invoke(applicationBuilder);
                if (openTelemetryExporterPrometheusAspNetCoreOptions.PrometheusScrapingEndpointConfigure != null)
                {
                    applicationBuilder.UseOpenTelemetryPrometheusScrapingEndpoint();
                }
            }

            return Task.CompletedTask;
        }

    }
}

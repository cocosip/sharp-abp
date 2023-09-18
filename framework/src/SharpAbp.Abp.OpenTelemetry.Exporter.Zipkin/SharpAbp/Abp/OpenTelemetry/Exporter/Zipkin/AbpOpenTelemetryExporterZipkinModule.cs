using global::OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry.Exporter.Zipkin
{
    [DependsOn(
        typeof(AbpOpenTelemetryModule)
        )]
    public class AbpOpenTelemetryExporterZipkinModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryExporterZipkinOptions>(options =>
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
            var openTelemetryExporterZipkinOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryExporterZipkinOptions>();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                var action = new Action<ZipkinExporterOptions>(c =>
                {
                    c.Endpoint = new Uri(openTelemetryExporterZipkinOptions.Endpoint);
                    c.UseShortTraceIds = openTelemetryExporterZipkinOptions.UseShortTraceIds;
                    c.MaxPayloadSizeInBytes = openTelemetryExporterZipkinOptions.MaxPayloadSizeInBytes;
                });

                options.TracingExporters.Add(OpenTelemetryExporterNames.Zipkin, new Action<TracerProviderBuilder>(builder =>
                {
                    builder.AddZipkinExporter(openTelemetryExporterZipkinOptions.Name, action);
                }));

            });

            return base.ConfigureServicesAsync(context);
        }
    }
}

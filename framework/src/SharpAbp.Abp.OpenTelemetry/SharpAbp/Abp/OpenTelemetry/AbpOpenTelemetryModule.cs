using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry
{
    [DependsOn(
        typeof(AbpOpenTelemetryAbstractionsModule)
        )]
    public class AbpOpenTelemetryModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                //read from configuration
                options.PreConfigure(configuration);

                options.SamplerConfigure = new Action<TracerProviderBuilder>(builder =>
                {
                    builder.SetSampler(new AlwaysOnSampler());
                });

                options.ExemplarFilterConfigure = new Action<MeterProviderBuilder>(builder =>
                {
#if EXPOSE_EXPERIMENTAL_FEATURES
                    builder.SetExemplarFilter();
#endif
                });
            });

            return Task.CompletedTask;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }


        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var openTelemetryOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryOptions>();
            if (openTelemetryOptions.WithTracing)
            {
                foreach (var keyValuePair in openTelemetryOptions.TracingExporters)
                {
                    if (openTelemetryOptions.UseTracingExporter.Equals(keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        PreConfigure<AbpOpenTelemetryOptions>(options =>
                        {
                            options.TracingExporter = keyValuePair.Value;
                        });
                        break;
                    }
                }
            }

            if (openTelemetryOptions.WithMetrics)
            {
                foreach (var keyValuePair in openTelemetryOptions.MetricsExporters)
                {
                    if (openTelemetryOptions.UseMetricsExporter.Equals(keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        PreConfigure<AbpOpenTelemetryOptions>(options =>
                        {
                            options.MetricsExporter = keyValuePair.Value;
                        });
                        break;
                    }
                }
            }

            if (openTelemetryOptions.WithLogging)
            {
                foreach (var keyValuePair in openTelemetryOptions.LoggingExporters)
                {
                    if (openTelemetryOptions.UseLoggingExporter.Equals(keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        PreConfigure<AbpOpenTelemetryOptions>(options =>
                        {
                            options.LoggingExporter = keyValuePair.Value;
                        });
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var preConfigureActions = context.Services.GetPreConfigureActions<AbpOpenTelemetryOptions>();
            Configure<AbpOpenTelemetryOptions>(options =>
            {
                foreach (var preConfigureAction in preConfigureActions)
                {
                    preConfigureAction(options);
                }
            });


            var openTelemetryBuilderOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryBuilderOptions>();
            var openTelemetryOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryOptions>();

            if (openTelemetryBuilderOptions.IsEnabled)
            {
                void configureResource(ResourceBuilder r) => r.AddService(
                    openTelemetryBuilderOptions.ServiceName,
                    openTelemetryBuilderOptions.ServiceVersion,
                    openTelemetryBuilderOptions.ServiceVersion,
                    openTelemetryBuilderOptions.AutoGenerateServiceInstanceId,
                    openTelemetryBuilderOptions.ServiceInstanceId);

                var openTelemetryBuilder = context.Services
                    .AddOpenTelemetry()
                    .ConfigureResource(configureResource);

                foreach (var preConfigure in openTelemetryOptions.OpenTelemetryBuilderPreConfigures)
                {
                    preConfigure?.Invoke(openTelemetryBuilder);
                }

                if (openTelemetryOptions.WithTracing)
                {
                    openTelemetryBuilder.WithTracing(builder =>
                    {
                        var sourceNames = openTelemetryOptions.SourceNames.ToArray();
                        if (sourceNames.Length == 0)
                        {
                            sourceNames = new string[] { "SharpAbp" };
                        }
                        //addSource
                        builder.AddSource(sourceNames);

                        //addLegacySource 
                        if (!openTelemetryOptions.OperationName.IsNullOrWhiteSpace())
                        {
                            builder.AddLegacySource(openTelemetryOptions.OperationName);
                        }

                        //setSampler
                        openTelemetryOptions.SamplerConfigure?.Invoke(builder);

                        //instrumentations
                        foreach (var instrumentationConfigure in openTelemetryOptions.TracingInstrumentationConfigures)
                        {
                            instrumentationConfigure?.Invoke(builder);
                        }

                        //exporter
                        openTelemetryOptions.TracingExporter?.Invoke(builder);

                        //Configure
                        foreach (var configure in openTelemetryOptions.TracingConfigures)
                        {
                            configure?.Invoke(builder);
                        }

                    });
                }

                if (openTelemetryOptions.WithMetrics)
                {
                    openTelemetryBuilder.WithMetrics(builder =>
                    {
                        var meterNames = openTelemetryOptions.MeterNames.ToArray();
                        if (meterNames.Length == 0)
                        {
                            meterNames = new string[] { "SharpAbp" };
                        }

                        //addMeter
                        builder.AddMeter(meterNames);

                        //setExemplarFilter
                        openTelemetryOptions.ExemplarFilterConfigure?.Invoke(builder);

                        //instrumentations
                        foreach (var instrumentationConfigure in openTelemetryOptions.MetricsInstrumentationConfigures)
                        {
                            instrumentationConfigure?.Invoke(builder);
                        }

                        //view
                        foreach (var viewConfigure in openTelemetryOptions.MetricsViewConfigures)
                        {
                            viewConfigure?.Invoke(builder);
                        }

                        //exporter
                        openTelemetryOptions.MetricsExporter?.Invoke(builder);

                        //configure
                        foreach (var configure in openTelemetryOptions.MetricsConfigures)
                        {
                            configure?.Invoke(builder);
                        }
                    });
                }

                if (openTelemetryOptions.WithLogging)
                {
                    context.Services.AddLogging(builder =>
                    {
                        if (openTelemetryOptions.LoggingExporter != null)
                        {
                            //ClearProviders
                            builder.ClearProviders();

                            builder.AddOpenTelemetry(options =>
                            {
                                var resourceBuilder = ResourceBuilder.CreateDefault();
                                configureResource(resourceBuilder);
                                options.SetResourceBuilder(resourceBuilder);

                                openTelemetryOptions.LoggingExporter?.Invoke(options);
                            });
                        }
                    });

                }

                foreach (var postConfigure in openTelemetryOptions.OpenTelemetryBuilderPostConfigures)
                {
                    postConfigure?.Invoke(openTelemetryBuilder);
                }
            }
            return Task.CompletedTask;
        }

    }
}

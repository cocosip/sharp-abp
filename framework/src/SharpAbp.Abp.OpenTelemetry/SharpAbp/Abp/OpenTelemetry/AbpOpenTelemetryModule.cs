using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry
{
    [DependsOn(typeof(AbpOpenTelemetryAbstractionsModule))]
    public class AbpOpenTelemetryModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryResourceOptions>(options =>
            {
                options.PreConfigure(configuration);
            });

            PreConfigure<AbpOpenTelemetryOptions>(options =>
            {
                options.PreConfigure(configuration);

                options.Tracing.SamplerConfigure = builder =>
                {
                    builder.SetSampler(new AlwaysOnSampler());
                };

                options.Metrics.ExemplarFilterConfigure = builder =>
                {
#if EXPOSE_EXPERIMENTAL_FEATURES
                    builder.SetExemplarFilter();
#endif
                };
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

            if (openTelemetryOptions.Tracing.IsEnabled &&
                TryResolveExporter(openTelemetryOptions.Tracing.ExporterName, openTelemetryOptions.TracingExporters, out var tracingExporter))
            {
                PreConfigure<AbpOpenTelemetryOptions>(options =>
                {
                    options.Tracing.ExporterConfigure = tracingExporter;
                });
            }

            if (openTelemetryOptions.Metrics.IsEnabled &&
                TryResolveExporter(openTelemetryOptions.Metrics.ExporterName, openTelemetryOptions.MetricsExporters, out var metricsExporter))
            {
                PreConfigure<AbpOpenTelemetryOptions>(options =>
                {
                    options.Metrics.ExporterConfigure = metricsExporter;
                });
            }

            if (openTelemetryOptions.Logging.IsEnabled &&
                TryResolveExporter(openTelemetryOptions.Logging.ExporterName, openTelemetryOptions.LoggingExporters, out var loggingExporter))
            {
                PreConfigure<AbpOpenTelemetryOptions>(options =>
                {
                    options.Logging.ExporterConfigure = loggingExporter;
                });
            }

            return Task.CompletedTask;
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var resourcePreConfigureActions = context.Services.GetPreConfigureActions<AbpOpenTelemetryResourceOptions>();
            Configure<AbpOpenTelemetryResourceOptions>(options =>
            {
                foreach (var preConfigureAction in resourcePreConfigureActions)
                {
                    preConfigureAction(options);
                }
            });

            var preConfigureActions = context.Services.GetPreConfigureActions<AbpOpenTelemetryOptions>();
            Configure<AbpOpenTelemetryOptions>(options =>
            {
                foreach (var preConfigureAction in preConfigureActions)
                {
                    preConfigureAction(options);
                }
            });

            var resourceOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryResourceOptions>();
            var openTelemetryOptions = context.Services.ExecutePreConfiguredActions<AbpOpenTelemetryOptions>();

            if (!resourceOptions.IsEnabled && !openTelemetryOptions.Resource.IsEnabled)
            {
                return Task.CompletedTask;
            }

            var serviceName = string.IsNullOrWhiteSpace(openTelemetryOptions.Resource.ServiceName)
                ? resourceOptions.ServiceName
                : openTelemetryOptions.Resource.ServiceName;
            serviceName = string.IsNullOrWhiteSpace(serviceName)
                ? (AppDomain.CurrentDomain.FriendlyName ?? "SharpAbp")
                : serviceName!;

            var serviceNamespace = string.IsNullOrWhiteSpace(openTelemetryOptions.Resource.ServiceNamespace)
                ? resourceOptions.ServiceNamespace
                : openTelemetryOptions.Resource.ServiceNamespace;

            var serviceVersion = string.IsNullOrWhiteSpace(openTelemetryOptions.Resource.ServiceVersion)
                ? resourceOptions.ServiceVersion
                : openTelemetryOptions.Resource.ServiceVersion;

            var autoGenerateServiceInstanceId =
                openTelemetryOptions.Resource.AutoGenerateServiceInstanceId || resourceOptions.AutoGenerateServiceInstanceId;

            var serviceInstanceId = string.IsNullOrWhiteSpace(openTelemetryOptions.Resource.ServiceInstanceId)
                ? resourceOptions.ServiceInstanceId
                : openTelemetryOptions.Resource.ServiceInstanceId;

            void ConfigureResource(ResourceBuilder resourceBuilder) => resourceBuilder.AddService(
                serviceName,
                serviceNamespace,
                serviceVersion,
                autoGenerateServiceInstanceId,
                serviceInstanceId);

            var openTelemetryBuilder = context.Services
                .AddOpenTelemetry()
                .ConfigureResource(ConfigureResource);

            foreach (var preConfigure in openTelemetryOptions.OpenTelemetryBuilderPreConfigures)
            {
                preConfigure?.Invoke(openTelemetryBuilder);
            }

            if (openTelemetryOptions.Tracing.IsEnabled)
            {
                openTelemetryBuilder.WithTracing(builder =>
                {
                    var sourceNames = openTelemetryOptions.Tracing.SourceNames.ToArray();
                    if (sourceNames.Length == 0)
                    {
                        sourceNames = [serviceName];
                    }

                    builder.AddSource(sourceNames);

                    openTelemetryOptions.Tracing.SamplerConfigure?.Invoke(builder);

                    foreach (var instrumentationConfigure in openTelemetryOptions.Tracing.InstrumentationConfigures)
                    {
                        instrumentationConfigure?.Invoke(builder);
                    }

                    openTelemetryOptions.Tracing.ExporterConfigure?.Invoke(builder);

                    foreach (var configure in openTelemetryOptions.Tracing.BuilderConfigures)
                    {
                        configure?.Invoke(builder);
                    }
                });
            }

            if (openTelemetryOptions.Metrics.IsEnabled)
            {
                openTelemetryBuilder.WithMetrics(builder =>
                {
                    var meterNames = openTelemetryOptions.Metrics.MeterNames.ToArray();
                    if (meterNames.Length == 0)
                    {
                        meterNames = [serviceName];
                    }

                    builder.AddMeter(meterNames);

                    openTelemetryOptions.Metrics.ExemplarFilterConfigure?.Invoke(builder);

                    foreach (var instrumentationConfigure in openTelemetryOptions.Metrics.InstrumentationConfigures)
                    {
                        instrumentationConfigure?.Invoke(builder);
                    }

                    foreach (var viewConfigure in openTelemetryOptions.Metrics.ViewConfigures)
                    {
                        viewConfigure?.Invoke(builder);
                    }

                    openTelemetryOptions.Metrics.ExporterConfigure?.Invoke(builder);

                    foreach (var configure in openTelemetryOptions.Metrics.BuilderConfigures)
                    {
                        configure?.Invoke(builder);
                    }
                });
            }

            if (openTelemetryOptions.Logging.IsEnabled)
            {
                context.Services.AddLogging(builder =>
                {
                    if (openTelemetryOptions.Logging.ClearProviders)
                    {
                        builder.ClearProviders();
                    }

                    if (openTelemetryOptions.Logging.ExporterConfigure != null)
                    {
                        builder.AddOpenTelemetry(options =>
                        {
                            var resourceBuilder = ResourceBuilder.CreateDefault();
                            ConfigureResource(resourceBuilder);
                            options.SetResourceBuilder(resourceBuilder);
                            options.IncludeFormattedMessage = openTelemetryOptions.Logging.IncludeFormattedMessage;
                            options.IncludeScopes = openTelemetryOptions.Logging.IncludeScopes;
                            options.ParseStateValues = openTelemetryOptions.Logging.ParseStateValues;

                            openTelemetryOptions.Logging.ExporterConfigure(options);

                            foreach (var configure in openTelemetryOptions.Logging.BuilderConfigures)
                            {
                                configure?.Invoke(options);
                            }
                        });
                    }
                });
            }

            foreach (var postConfigure in openTelemetryOptions.OpenTelemetryBuilderPostConfigures)
            {
                postConfigure?.Invoke(openTelemetryBuilder);
            }

            return Task.CompletedTask;
        }

        private static bool TryResolveExporter<TBuilder>(
            string? exporterName,
            IDictionary<string, Action<TBuilder>> exporters,
            out Action<TBuilder>? exporter)
        {
            exporter = default;
            if (string.IsNullOrWhiteSpace(exporterName))
            {
                return false;
            }

            foreach (var keyValuePair in exporters)
            {
                if (string.Equals(exporterName, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    exporter = keyValuePair.Value;
                    return true;
                }
            }

            return false;
        }
    }
}


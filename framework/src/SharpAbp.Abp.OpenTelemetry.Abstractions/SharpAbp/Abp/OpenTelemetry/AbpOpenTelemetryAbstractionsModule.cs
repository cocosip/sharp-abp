using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.OpenTelemetry
{
    public class AbpOpenTelemetryAbstractionsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpOpenTelemetryBuilderOptions>(options =>
            {
                options.PreConfigure(configuration);
            });

            return Task.CompletedTask;
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var actions = context.Services.GetPreConfigureActions<AbpOpenTelemetryBuilderOptions>();
            Configure<AbpOpenTelemetryBuilderOptions>(options =>
            {
                foreach (var action in actions)
                {
                    action(options);
                }
            });
            return Task.CompletedTask;
        }

    }
}

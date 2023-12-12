using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.EventBus.MassTransit
{
    [DependsOn(
        typeof(AbpEventBusModule),
        typeof(AbpMassTransitModule)
        )]
    public class AbpEventBusMassTransitModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitEventBusOptions>(options =>
            {
                options.Topic = "Abp.MassTransit.EventBus";
            });

            Configure<AbpMassTransitEventBusOptions>(options =>
            {
                options.Topic = "Abp.MassTransit.EventBus";
            });

            return Task.CompletedTask;
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var actions = context.Services.GetPreConfigureActions<AbpMassTransitEventBusOptions>();
            var options = new AbpMassTransitEventBusOptions();

            foreach (var action in actions)
            {
                action(options);
            }

            return Task.CompletedTask;
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
        }

        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            context
            .ServiceProvider
            .GetRequiredService<MassTransitDistributedEventBus>()
            .Initialize();

            return Task.CompletedTask;
        }

    }
}

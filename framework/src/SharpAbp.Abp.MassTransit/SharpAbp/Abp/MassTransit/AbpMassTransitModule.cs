using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MassTransit
{
    public class AbpMassTransitModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpMassTransitOptions>(options =>
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
            Configure<AbpMassTransitOptions>(options =>
            {
                var actions = context.Services.GetPreConfigureActions<AbpMassTransitOptions>();
                foreach (var action in actions)
                {
                    action(options);
                }
            });

            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();
            var startTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StartTimeoutMilliSeconds);
            var stopTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StopTimeoutMilliSeconds);

            Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = massTransitOptions.WaitUntilStarted;
                options.StartTimeout = startTimeout;
                options.StopTimeout = stopTimeout;
            });
            return Task.CompletedTask;
        }

    }
}

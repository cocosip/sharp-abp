using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    public class AbpMassTransitModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpMassTransitOptions>(options =>
            {
                options.PreConfigure(configuration);
            });
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            var startTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StartTimeoutMilliSeconds);

            var stopTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StopTimeoutMilliSeconds);

            context.Services.AddMassTransitHostedService(massTransitOptions.WaitUntilStarted, startTimeout, stopTimeout);


            Configure<AbpMassTransitOptions>(options =>
            {
                var actions = context.Services.GetPreConfigureActions<AbpMassTransitOptions>();
                foreach (var action in actions)
                {
                    action(options);
                }
            });

        }
    }
}

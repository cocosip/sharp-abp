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
            PreConfigure<AbpMassTransitOptions>(options =>
            {
                options.Prefix = "SharpAbp";
                options.StartTimeoutMilliSeconds = 30000;
                options.StopTimeoutMilliSeconds = 5000;
            });
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            var startTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StartTimeoutMilliSeconds);

            var stopTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StopTimeoutMilliSeconds);

            context.Services.AddMassTransitHostedService(massTransitOptions.WaitUntilStarted, startTimeout, stopTimeout);
        }
    }
}

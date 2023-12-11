using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    public class MassTransitSetupUtil
    {
        public static void ConfigureMassTransitHost(ServiceConfigurationContext context)
        {
            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();
            var startTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StartTimeoutMilliSeconds);
            var stopTimeout = TimeSpan.FromMilliseconds(massTransitOptions.StopTimeoutMilliSeconds);

            context.Services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = massTransitOptions.WaitUntilStarted;
                options.StartTimeout = startTimeout;
                options.StopTimeout = stopTimeout;
            });
        }

    }
}

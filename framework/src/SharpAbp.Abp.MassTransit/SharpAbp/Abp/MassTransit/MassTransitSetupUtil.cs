using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Utility class for configuring MassTransit host options in ABP applications
    /// </summary>
    public static class MassTransitSetupUtil
    {
        /// <summary>
        /// Configures MassTransit host options based on ABP MassTransit configuration
        /// </summary>
        /// <param name="context">The service configuration context</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when configuration is invalid</exception>
        public static void ConfigureMassTransitHost(ServiceConfigurationContext context)
        {
            Check.NotNull(context, nameof(context));

            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();
            
            if (!massTransitOptions.IsValid())
            {
                throw new InvalidOperationException(
                    $"Invalid MassTransit configuration: StartTimeout must be between 1-300 seconds, " +
                    $"StopTimeout must be between 1-60 seconds. " +
                    $"Current values: StartTimeout={massTransitOptions.StartTimeoutMilliSeconds}ms, " +
                    $"StopTimeout={massTransitOptions.StopTimeoutMilliSeconds}ms");
            }

            var startTimeout = massTransitOptions.GetStartTimeout();
            var stopTimeout = massTransitOptions.GetStopTimeout();

            context.Services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = massTransitOptions.WaitUntilStarted;
                options.StartTimeout = startTimeout;
                options.StopTimeout = stopTimeout;
            });
        }
    }
}

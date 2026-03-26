using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// ABP module for MassTransit integration providing core messaging functionality
    /// </summary>
    public class AbpMassTransitModule : AbpModule
    {
        /// <summary>
        /// Pre-configures services for the MassTransit module
        /// </summary>
        /// <param name="context">The service configuration context</param>
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        /// <summary>
        /// Pre-configures services asynchronously for the MassTransit module
        /// </summary>
        /// <param name="context">The service configuration context</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpMassTransitOptions>(options =>
            {
                options.Prefix = "SharpAbp";
                options.PreConfigure(configuration);

                options.PreConfigures.Add(c =>
                {
                    c.DisableUsageTelemetry();
                });
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Post-configures services for the MassTransit module
        /// </summary>
        /// <param name="context">The service configuration context</param>
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PostConfigureServicesAsync(context));
        }

        /// <summary>
        /// Post-configures services asynchronously for the MassTransit module
        /// </summary>
        /// <param name="context">The service configuration context</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public override Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var preConfiguredOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();
            Configure<AbpMassTransitOptions>(options => options.CopyFrom(preConfiguredOptions));

            // Register the default publisher
            context.Services.AddTransient<IMassTransitPublisher, DefaultMassTransitPublisher>();

            return Task.CompletedTask;
        }
    }
}

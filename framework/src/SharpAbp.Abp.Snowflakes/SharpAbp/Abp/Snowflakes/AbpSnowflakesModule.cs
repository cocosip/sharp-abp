using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// The main module for the SharpAbp.Abp.Snowflakes library.
    /// This module configures the default Snowflake options.
    /// </summary>
    public class AbpSnowflakesModule : AbpModule
    {
        /// <summary>
        /// Configures the services for the module synchronously.
        /// This method is called by the ABP framework during module initialization.
        /// </summary>
        /// <param name="context">The service configuration context.</param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // Synchronously run the asynchronous configuration to ensure services are set up before continuing.
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        /// <summary>
        /// Configures the services for the module asynchronously.
        /// This method is called by the ABP framework during module initialization.
        /// </summary>
        /// <param name="context">The service configuration context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            // Configure the AbpSnowflakesOptions to set default worker and datacenter IDs.
            Configure<AbpSnowflakesOptions>(options =>
            {
                options.Snowflakes.ConfigureDefault(c =>
                {
                    c.WorkerId = 1L;
                    c.DatacenterId = 1L;
                });
            });
            return Task.CompletedTask;
        }
    }
}

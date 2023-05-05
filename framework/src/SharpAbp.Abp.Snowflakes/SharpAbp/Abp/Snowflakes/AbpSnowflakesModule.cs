using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Snowflakes
{
    public class AbpSnowflakesModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
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

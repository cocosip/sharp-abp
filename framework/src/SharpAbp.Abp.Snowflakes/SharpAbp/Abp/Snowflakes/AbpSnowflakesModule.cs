using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Snowflakes
{
    public class AbpSnowflakesModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpSnowflakesOptions>(options =>
            {
                options.Snowflakes.ConfigureDefault(c =>
                {
                    c.WorkerId = 1L;
                    c.DatacenterId = 1L;
                });
            });
        }
    }
}

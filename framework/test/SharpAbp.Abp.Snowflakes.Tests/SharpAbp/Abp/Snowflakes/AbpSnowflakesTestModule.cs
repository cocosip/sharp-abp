using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Snowflakes
{
    [DependsOn(
      typeof(AbpSnowflakesModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpSnowflakesTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AbpSnowflakesOptions>(options =>
            {
                options.Configure(configuration);

                options.Snowflakes.Configure<DefaultSnowflake>(c =>
                {
                    c.DatacenterId = 3;
                    c.WorkerId = 3;
                });
            });


        }
    }
}

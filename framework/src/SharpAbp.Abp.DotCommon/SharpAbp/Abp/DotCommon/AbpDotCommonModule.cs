using DotCommon.DependencyInjection;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DotCommon
{
    public class AbpDotCommonModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddDotCommon();
            Configure<SnowflakeIdOptions>(options =>
            {
                options.WorkerId = 1L;
                options.DatacenterId = 1L;
            });
        }

    }
}

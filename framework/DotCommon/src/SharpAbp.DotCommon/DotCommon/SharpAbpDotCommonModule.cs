using DotCommon.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.DotCommon.Snowflake;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon
{
    /// <summary>SharpAbp.DotCommon模块
    /// </summary>
    public class SharpAbpDotCommonModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services
                .AddDotCommon()
                .Configure<SnowflakeIdGeneratorOption>(o =>
                {
                    o.DataCenterId = 1L;
                    o.WorkerId = 1L;
                })
                .AddSingleton<SnowflakeIdGenerator>();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureDotCommon();
        }

    }
}

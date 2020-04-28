using DotCommon.AutoMapper;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon.AutoMapper
{
    [DependsOn(typeof(SharpAbpDotCommonModule))]
    public class SharpAbpDotCommonAutoMapperModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddDotCommonAutoMapper();
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.BuildAutoMapper();
        }
    }
}

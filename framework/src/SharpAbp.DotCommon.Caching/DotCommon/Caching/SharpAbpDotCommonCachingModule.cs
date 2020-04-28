using DotCommon.Caching;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon.Caching
{
    [DependsOn(typeof(SharpAbpDotCommonModule))]
    public class SharpAbpDotCommonCachingModule: AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddGenericsMemoryCache();
        }
    }
}

using DotCommon.ImageResizer.AspNetCore.Mvc;
using SharpAbp.DotCommon.Caching;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon.ImageResizer.AspNetCore.Mvc
{
    [DependsOn(typeof(SharpAbpDotCommonCachingModule))]
    public class SharpAbpDotCommonImageResizerAspNetCoreMvcModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddImageResizer();
        }



    }
}

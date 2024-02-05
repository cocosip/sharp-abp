using SharpAbp.Abp.AspNetCore.FrontHost;
using SharpAbp.Abp.AspNetCore.Http;
using SharpAbp.Abp.AspNetCore.Response;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.AspNetCore
{
    [DependsOn(
        typeof(AbpAspNetCoreModule)
        )]
    public class SharpAbpAspNetCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpHttpResponseHeaderOptions>(options => { });
            Configure<AbpFrontHostOptions>(options => { });
            Configure<AbpHttpHeadersOptions>(options =>
            {
                options.RouteTranslationPrefix = "X-Abp";
            });
            return Task.CompletedTask;
        }
    }
}

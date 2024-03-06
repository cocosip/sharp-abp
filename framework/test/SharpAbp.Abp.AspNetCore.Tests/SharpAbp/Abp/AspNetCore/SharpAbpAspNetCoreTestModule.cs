using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.AspNetCore.FrontHost;
using SharpAbp.Abp.AspNetCore.Response;
using Volo.Abp;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AspNetCore
{
    [DependsOn(
        typeof(AbpAspNetCoreTestBaseModule),
        typeof(SharpAbpAspNetCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class SharpAbpAspNetCoreTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var webHostEnvironment = context.Services.GetHostingEnvironment();

            Configure<AbpHttpResponseHeaderOptions>(options => options.Configure(configuration));
            Configure<AbpFrontHostOptions>(options => options.Configure(configuration, webHostEnvironment.ContentRootPath));
        }

    }
}

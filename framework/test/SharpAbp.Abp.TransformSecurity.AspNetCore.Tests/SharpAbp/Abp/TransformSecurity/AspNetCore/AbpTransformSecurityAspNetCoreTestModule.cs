using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    [DependsOn(
        typeof(AbpAspNetCoreTestBaseModule),
        typeof(AbpTransformSecurityAspNetCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
    )]
    public class AbpTransformSecurityAspNetCoreTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var webHostEnvironment = context.Services.GetHostingEnvironment();

            //Configure<AbpHttpResponseHeaderOptions>(options => options.Configure(configuration));
            //Configure<AbpFrontHostOptions>(options => options.Configure(configuration, webHostEnvironment.ContentRootPath));
        }

    }
}

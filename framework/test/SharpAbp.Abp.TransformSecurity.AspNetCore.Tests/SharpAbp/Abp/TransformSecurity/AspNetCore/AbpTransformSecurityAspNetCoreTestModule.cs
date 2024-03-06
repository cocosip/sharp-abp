using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

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
            //var configuration = context.Services.GetConfiguration();
            //var webHostEnvironment = context.Services.GetHostingEnvironment();

            //Configure<AbpHttpResponseHeaderOptions>(options => options.Configure(configuration));
            //Configure<AbpFrontHostOptions>(options => options.Configure(configuration, webHostEnvironment.ContentRootPath));

            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            return base.ConfigureServicesAsync(context);
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
        }

        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var applicationBuilder = context.GetApplicationBuilder();
            applicationBuilder.UseAbpTransformSecurity();
            return Task.CompletedTask;
        }

    }
}

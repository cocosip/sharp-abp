using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    [DependsOn(
        typeof(AbpTransformSecurityModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class AbpTransformSecurityAspNetCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {

            Configure<AbpTransformSecurityAspNetCoreOptions>(options =>
            {
                options.MiddlewareHandlers.Add<TokenAuthHandler>();
            });

            return Task.CompletedTask;
        }
    }
}

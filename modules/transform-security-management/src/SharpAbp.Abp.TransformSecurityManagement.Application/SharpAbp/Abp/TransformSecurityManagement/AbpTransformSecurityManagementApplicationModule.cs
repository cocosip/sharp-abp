using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [DependsOn(
        typeof(AbpTransformSecurityManagementApplicationContractsModule),
        typeof(AbpTransformSecurityManagementDomainModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class AbpTransformSecurityManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<AbpTransformSecurityManagementApplicationModule>();
            return Task.CompletedTask;
        }
    }
}

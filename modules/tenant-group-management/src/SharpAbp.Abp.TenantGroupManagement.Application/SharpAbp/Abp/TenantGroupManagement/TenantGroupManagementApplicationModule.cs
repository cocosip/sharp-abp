using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(TenantGroupManagementDomainModule),
        typeof(TenantGroupManagementApplicationContractsModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class TenantGroupManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<TenantGroupManagementApplicationModule>();
            return Task.CompletedTask;
        }

    }
}

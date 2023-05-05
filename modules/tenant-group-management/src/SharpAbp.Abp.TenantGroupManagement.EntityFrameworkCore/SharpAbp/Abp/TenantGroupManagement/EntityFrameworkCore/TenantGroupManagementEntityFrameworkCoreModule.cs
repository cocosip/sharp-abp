using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(TenantGroupManagementDomainModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class TenantGroupManagementEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<TenantGroupManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<ITenantGroupManagementDbContext>(includeAllEntities: true);
                options.AddRepository<TenantGroup, EfCoreTenantGroupRepository>();
            });
            return Task.CompletedTask;
        }

    }
}

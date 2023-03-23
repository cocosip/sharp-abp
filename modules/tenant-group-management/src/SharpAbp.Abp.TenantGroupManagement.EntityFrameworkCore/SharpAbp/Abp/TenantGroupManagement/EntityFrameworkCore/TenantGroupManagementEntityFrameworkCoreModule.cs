using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

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
            context.Services.AddAbpDbContext<TenantGroupManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<ITenantGroupManagementDbContext>();
                options.AddRepository<TenantGroup, EfCoreTenantGroupRepository>();
            });
        }
    }
}

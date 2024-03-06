using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpTransformSecurityManagementDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class AbpTransformSecurityManagementEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() =>
            {
                return ConfigureServicesAsync(context);
            });
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpTransformSecurityManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<IAbpTransformSecurityManagementDbContext>(includeAllEntities: true);
                options.AddRepository<SecurityCredentialInfo, EfCoreSecurityCredentialInfoRepository>();
 
            });

            return Task.CompletedTask;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurityManagement.MongoDB
{
    [DependsOn(
        typeof(AbpTransformSecurityManagementDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class AbpTransformSecurityManagementMongoDbModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<AbpTransformSecurityManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IAbpTransformSecurityManagementMongoDbContext>();
                options.AddRepository<SecurityCredentialInfo, MongoDbSecurityCredentialInfoRepository>();
            });
            return Task.CompletedTask;
        }

    }
}

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [DependsOn(
        typeof(DbConnectionsManagementApplicationContractsModule),
        typeof(DbConnectionsManagementDomainModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class DbConnectionsManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<DbConnectionsManagementApplicationModule>();
            return Task.CompletedTask;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [DependsOn(
        typeof(DbConnectionsManagementApplicationContractsModule),
        typeof(DbConnectionsManagementDomainModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    public class DbConnectionsManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<DbConnectionsManagementApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<DbConnectionsManagementApplicationModule>();
            return Task.CompletedTask;
        }
    }
}

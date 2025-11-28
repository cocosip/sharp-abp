using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.MinId;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace MinIdApp
{
    [DependsOn(
        typeof(MinIdAppDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(MinIdAppApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule),
        typeof(MinIdApplicationModule)
        )]
    public class MinIdAppApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }


        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<MinIdAppApplicationModule>();
            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TenancyGrouping.ConfigurationStore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TenancyGrouping
{
    [DependsOn(
        typeof(AbpMultiTenancyModule),
        typeof(AbpTenancyGroupingAbstractionsModule)
        )]
    public class AbpTenancyGroupingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpTenancyGroupingOptions>(options =>
            {
                options.IsEnabled = true;
            });

            //添加默认TenantGroup解析
            Configure<AbpTenantGroupResolveOptions>(options =>
            {
                options.TenantResolvers.Add(new DefaultTenantGroupResolveContributor());
            });


            var configuration = context.Services.GetConfiguration();
            Configure<AbpDefaultTenantGroupStoreOptions>(configuration);

            context.Services.AddSingleton<ICurrentTenantGroupAccessor>(AsyncLocalCurrentTenantGroupAccessor.Instance);

            return Task.CompletedTask;
        }
    }
}

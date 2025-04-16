using System.Threading.Tasks;
using SharpAbp.Abp.TenancyGrouping.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.Localization;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.TenancyGrouping
{
    [DependsOn(
        typeof(AbpMultiTenancyAbstractionsModule)
        )]
    public class AbpTenancyGroupingAbstractionsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpTenancyGroupingAbstractionsModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<AbpTenancyGroupingResource>("en")
                    .AddVirtualJson("/SharpAbp/Abp/TenancyGrouping/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("AbpTenancyGrouping", typeof(AbpTenancyGroupingResource));
            });

            return Task.CompletedTask;
        }

    }
}

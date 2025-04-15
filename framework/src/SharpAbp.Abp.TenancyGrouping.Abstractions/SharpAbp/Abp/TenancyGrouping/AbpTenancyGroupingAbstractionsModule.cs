using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

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
            //Configure<AbpVirtualFileSystemOptions>(options =>
            //{
            //    options.FileSets.AddEmbedded<AbpMultiTenancyAbstractionsModule>();
            //});

            //Configure<AbpLocalizationOptions>(options =>
            //{
            //    options.Resources
            //        .Add<AbpMultiTenancyResource>("en")
            //        .AddVirtualJson("/Volo/Abp/MultiTenancy/Localization");
            //});

            return Task.CompletedTask;
        }

    }
}

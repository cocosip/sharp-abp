using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.Swashbuckle
{
    [DependsOn(
         typeof(AbpSwashbuckleModule)
         )]
    public class SharpAbpSwashbuckleModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SharpAbpSwashbuckleModule>();
            });
            return Task.CompletedTask;
        }

    }
}

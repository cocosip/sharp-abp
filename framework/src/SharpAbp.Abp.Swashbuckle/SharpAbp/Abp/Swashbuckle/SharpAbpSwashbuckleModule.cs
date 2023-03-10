using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
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
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SharpAbpSwashbuckleModule>();
            });
        }
    }
}

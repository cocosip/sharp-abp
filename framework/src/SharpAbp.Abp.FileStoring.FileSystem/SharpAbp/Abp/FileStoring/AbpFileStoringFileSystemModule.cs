using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
       typeof(AbpFileStoringModule)
       )]
    public class AbpFileStoringFileSystemModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(c =>
            {
                //c.Providers.TryAdd(new FileProviderConfiguration(typeof(FileSystemFileProvider), typeof(FileSystemFileNamingNormalizer)));
            });
        }
    }
}

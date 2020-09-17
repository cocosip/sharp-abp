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
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(typeof(FileSystemFileProvider));
            configuration.DefaultNamingNormalizers.TryAdd<FileSystemFileNamingNormalizer>();
            configuration
                .SetProperty(FileSystemFileProviderConfigurationNames.BasePath, typeof(string))
                .SetProperty(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, typeof(string))
                .SetProperty(FileSystemFileProviderConfigurationNames.HttpServer, typeof(string));
            return configuration;
        }
    }
}

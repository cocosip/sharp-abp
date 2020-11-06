using SharpAbp.Abp.FileStoring.FileSystem.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    [DependsOn(
       typeof(AbpFileStoringModule),
       typeof(AbpValidationModule)
    )]
    public class AbpFileStoringFileSystemModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpFileStoringFileSystemModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringFileSystemResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/FileSystem/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringFastDFS", typeof(FileStoringFileSystemResource));
            });
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(FileSystemFileProviderConfigurationNames.ProviderName);
            configuration.DefaultNamingNormalizers.TryAdd<FileSystemFileNamingNormalizer>();
            configuration
                .SetProperty(FileSystemFileProviderConfigurationNames.BasePath, typeof(string))
                .SetProperty(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, typeof(bool))
                .SetProperty(FileSystemFileProviderConfigurationNames.HttpServer, typeof(string));
            return configuration;
        }
    }
}

using SharpAbp.Abp.FileStoring.FileSystem.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    [DependsOn(
        typeof(AbpFileStoringModule)
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
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/FileSystem/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringFastDFS", typeof(FileStoringFileSystemResource));
            });
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                FileSystemFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringFileSystemResource));
                
            configuration.DefaultNamingNormalizers.TryAdd<FileSystemFileNamingNormalizer>();
            configuration
                .SetValueType(FileSystemFileProviderConfigurationNames.BasePath, typeof(string))
                .SetValueType(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, typeof(bool))
                .SetValueType(FileSystemFileProviderConfigurationNames.HttpServer, typeof(string));
            return configuration;
        }
    }
}

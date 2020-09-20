using SharpAbp.Abp.FileStoring.Azure.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.Azure
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpValidationModule)
    )]
    public class AbpFileStoringAzureModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpFileStoringAzureModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringAzureResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Azure/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringAzure", typeof(FileStoringAzureResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(typeof(AzureFileProvider));
            configuration.DefaultNamingNormalizers.TryAdd<AzureFileNamingNormalizer>();
            configuration
                .SetProperty(AzureFileProviderConfigurationNames.ConnectionString, typeof(string))
                .SetProperty(AzureFileProviderConfigurationNames.ContainerName, typeof(string))
                .SetProperty(AzureFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool));
            return configuration;
        }
    }
}

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
        typeof(AbpFileStoringModule)
        )]
    public class AbpFileStoringAzureModule : AbpModule
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
            var configuration = new FileProviderConfiguration(AzureFileProviderConfigurationNames.ProviderName);
            configuration.DefaultNamingNormalizers.TryAdd<AzureFileNamingNormalizer>();
            configuration
                .SetValue(AzureFileProviderConfigurationNames.ConnectionString, typeof(string))
                .SetValue(AzureFileProviderConfigurationNames.ContainerName, typeof(string))
                .SetValue(AzureFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool));
            return configuration;
        }
    }
}

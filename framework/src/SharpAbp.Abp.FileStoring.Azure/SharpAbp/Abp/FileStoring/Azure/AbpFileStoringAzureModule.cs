using SharpAbp.Abp.FileStoring.Azure.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
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
            PreConfigure<AbpFileStoringAbstractionsOptions>(c =>
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
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Azure/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringAzure", typeof(FileStoringAzureResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                AzureFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringAzureResource));

            configuration.DefaultNamingNormalizers.TryAdd<AzureFileNamingNormalizer>();
            configuration
                .AddItem(AzureFileProviderConfigurationNames.ConnectionString, typeof(string), "DefaultEndpointsProtocol=[http|https];AccountName=myAccountName;AccountKey=myAccountKey")
                .AddItem(AzureFileProviderConfigurationNames.ContainerName, typeof(string), "container1")
                .AddItem(AzureFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool), "false");
            return configuration;
        }
    }
}

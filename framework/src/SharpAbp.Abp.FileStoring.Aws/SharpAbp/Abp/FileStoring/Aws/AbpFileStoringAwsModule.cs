using SharpAbp.Abp.FileStoring.Aws.Localization;
using Volo.Abp.Caching;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.Aws
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpCachingModule)
        )]
    public class AbpFileStoringAwsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpFileStoringAbstractionsOptions>(options =>
            {
                var configuration = GetFileProviderConfiguration();
                options.Providers.TryAdd(configuration);
            });
        }


        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpFileStoringAwsModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringAwsResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Aws/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringAws", typeof(FileStoringAwsResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                AwsFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringAwsResource));

            configuration.DefaultNamingNormalizers.TryAdd<AwsFileNamingNormalizer>();
            configuration
                .AddItem(AwsFileProviderConfigurationNames.AccessKeyId, typeof(string), "ak")
                .AddItem(AwsFileProviderConfigurationNames.SecretAccessKey, typeof(string), "sk")
                .AddItem(AwsFileProviderConfigurationNames.UseCredentials, typeof(bool), "false")
                .AddItem(AwsFileProviderConfigurationNames.UseTemporaryCredentials, typeof(bool), "false")
                .AddItem(AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials, typeof(bool), "false")
                .AddItem(AwsFileProviderConfigurationNames.ProfileName, typeof(string), "")
                .AddItem(AwsFileProviderConfigurationNames.ProfilesLocation, typeof(string), "")
                .AddItem(AwsFileProviderConfigurationNames.DurationSeconds, typeof(int), "3600")
                .AddItem(AwsFileProviderConfigurationNames.Name, typeof(string), "bucket1")
                .AddItem(AwsFileProviderConfigurationNames.Policy, typeof(string), "")
                .AddItem(AwsFileProviderConfigurationNames.Region, typeof(string), "eu-west-1")
                .AddItem(AwsFileProviderConfigurationNames.ContainerName, typeof(string), "container1")
                .AddItem(AwsFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool), "false")
                .AddItem(AwsFileProviderConfigurationNames.TemporaryCredentialsCacheKey, typeof(string), "AwsCacheKey");

            return configuration;
        }
    }
}

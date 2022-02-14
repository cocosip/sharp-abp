using SharpAbp.Abp.FileStoring.KS3.Localization;
using Volo.Abp.Caching;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.KS3
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpCachingModule)
        )]
    public class AbpFileStoringKS3Module : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(options =>
            {
                var configuration = GetFileProviderConfiguration();
                options.Providers.TryAdd(configuration);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpFileStoringKS3Module>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringKS3Resource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/KS3/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringKS3", typeof(FileStoringKS3Resource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                KS3FileProviderConfigurationNames.ProviderName,
                typeof(FileStoringKS3Resource));

            configuration.DefaultNamingNormalizers.TryAdd<KS3FileNamingNormalizer>();
            configuration
                .SetValueType(KS3FileProviderConfigurationNames.AccessKey, typeof(string))
                .SetValueType(KS3FileProviderConfigurationNames.SecretKey, typeof(string))
                .SetValueType(KS3FileProviderConfigurationNames.BucketName, typeof(string))
                .SetValueType(KS3FileProviderConfigurationNames.Endpoint, typeof(string))
                .SetValueType(KS3FileProviderConfigurationNames.UserAgent, typeof(string))
                .SetValueType(KS3FileProviderConfigurationNames.Protocol, typeof(string))
                .SetValueType(KS3FileProviderConfigurationNames.MaxConnections, typeof(int))
                .SetValueType(KS3FileProviderConfigurationNames.Timeout, typeof(int))
                .SetValueType(KS3FileProviderConfigurationNames.ReadWriteTimeout, typeof(int))
                .SetValueType(KS3FileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool));

            return configuration;
        }
    }
}

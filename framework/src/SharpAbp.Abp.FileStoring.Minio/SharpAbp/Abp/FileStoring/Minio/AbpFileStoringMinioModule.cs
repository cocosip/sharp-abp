using SharpAbp.Abp.FileStoring.Minio.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.Minio
{
    [DependsOn(
        typeof(AbpFileStoringModule)
        )]
    public class AbpFileStoringMinioModule : AbpModule
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
                options.FileSets.AddEmbedded<AbpFileStoringMinioModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringMinioResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Minio/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringMinio", typeof(FileStoringMinioResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(MinioFileProviderConfigurationNames.ProviderName);
            configuration.DefaultNamingNormalizers.TryAdd<MinioFileNamingNormalizer>();
            configuration
                .SetValue(MinioFileProviderConfigurationNames.BucketName, typeof(string))
                .SetValue(MinioFileProviderConfigurationNames.EndPoint, typeof(string))
                .SetValue(MinioFileProviderConfigurationNames.AccessKey, typeof(string))
                .SetValue(MinioFileProviderConfigurationNames.SecretKey, typeof(string))
                .SetValue(MinioFileProviderConfigurationNames.WithSSL, typeof(bool))
                .SetValue(MinioFileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool));

            return configuration;
        }
    }
}

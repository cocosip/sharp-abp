﻿using SharpAbp.Abp.FileStoring.Minio.Localization;
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
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Minio/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringMinio", typeof(FileStoringMinioResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                MinioFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringMinioResource));

            configuration.DefaultNamingNormalizers.TryAdd<MinioFileNamingNormalizer>();
            configuration
                .SetValueType(MinioFileProviderConfigurationNames.BucketName, typeof(string), "bucket1")
                .SetValueType(MinioFileProviderConfigurationNames.EndPoint, typeof(string), "192.168.0.100:9005")
                .SetValueType(MinioFileProviderConfigurationNames.AccessKey, typeof(string), "minioadmin")
                .SetValueType(MinioFileProviderConfigurationNames.SecretKey, typeof(string), "minioadmin")
                .SetValueType(MinioFileProviderConfigurationNames.WithSSL, typeof(bool), "false")
                .SetValueType(MinioFileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool), "false");

            return configuration;
        }
    }
}

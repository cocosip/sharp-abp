﻿using System.Threading.Tasks;
using SharpAbp.Abp.FileStoring.Minio.Localization;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.Minio
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpObjectPoolModule)
        )]
    public class AbpFileStoringMinioModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<AbpFileStoringAbstractionsOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });
            return Task.CompletedTask;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
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

            return Task.CompletedTask;
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                MinioFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringMinioResource));

            configuration.DefaultNamingNormalizers.TryAdd<MinioFileNamingNormalizer>();
            configuration
                .AddItem(MinioFileProviderConfigurationNames.BucketName, typeof(string), "bucket1")
                .AddItem(MinioFileProviderConfigurationNames.EndPoint, typeof(string), "192.168.0.100:9005")
                .AddItem(MinioFileProviderConfigurationNames.AccessKey, typeof(string), "minioadmin")
                .AddItem(MinioFileProviderConfigurationNames.SecretKey, typeof(string), "minioadmin")
                .AddItem(MinioFileProviderConfigurationNames.WithSSL, typeof(bool), "false")
                .AddItem(MinioFileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool), "false");

            return configuration;
        }
    }
}

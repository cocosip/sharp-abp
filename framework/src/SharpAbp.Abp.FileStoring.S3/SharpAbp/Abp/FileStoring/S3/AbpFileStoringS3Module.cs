using SharpAbp.Abp.AmazonS3.KS3;
using SharpAbp.Abp.FileStoring.S3.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.S3
{
    [DependsOn(
      typeof(AbpFileStoringModule),
      typeof(AbpAmazonS3KS3Module),
      typeof(AbpValidationModule)
    )]
    public class AbpFileStoringS3Module : AbpModule
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
                options.FileSets.AddEmbedded<AbpFileStoringS3Module>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringS3Resource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/S3/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringS3", typeof(FileStoringS3Resource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(typeof(S3FileProvider));
            configuration.DefaultNamingNormalizers.TryAdd<S3FileNamingNormalizer>();
            configuration
                .SetProperty(S3FileProviderConfigurationNames.BucketName, typeof(string))
                .SetProperty(S3FileProviderConfigurationNames.ServerUrl, typeof(string))
                .SetProperty(S3FileProviderConfigurationNames.AccessKeyId, typeof(string))
                .SetProperty(S3FileProviderConfigurationNames.SecretAccessKey, typeof(string))
                .SetProperty(S3FileProviderConfigurationNames.ForcePathStyle, typeof(bool))
                .SetProperty(S3FileProviderConfigurationNames.UseChunkEncoding, typeof(bool))
                .SetProperty(S3FileProviderConfigurationNames.Protocol, typeof(int))
                .SetProperty(S3FileProviderConfigurationNames.VendorType, typeof(int))
                .SetProperty(S3FileProviderConfigurationNames.EnableSlice, typeof(bool))
                .SetProperty(S3FileProviderConfigurationNames.SliceSize, typeof(int))
                .SetProperty(S3FileProviderConfigurationNames.SignatureVersion, typeof(string))
                .SetProperty(S3FileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool))
                .SetProperty(S3FileProviderConfigurationNames.ClientCount, typeof(int));

            return configuration;
        }
    }
}

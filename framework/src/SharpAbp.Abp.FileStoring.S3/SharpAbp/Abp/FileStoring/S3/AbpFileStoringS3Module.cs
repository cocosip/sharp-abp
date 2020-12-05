using SharpAbp.Abp.AutoS3.KS3;
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
      typeof(AbpAutoS3KS3Module),
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
            var configuration = new FileProviderConfiguration(S3FileProviderConfigurationNames.ProviderName);
            configuration.DefaultNamingNormalizers.TryAdd<S3FileNamingNormalizer>();
            configuration
                .SetValue(S3FileProviderConfigurationNames.BucketName, typeof(string))
                .SetValue(S3FileProviderConfigurationNames.ServerUrl, typeof(string))
                .SetValue(S3FileProviderConfigurationNames.AccessKeyId, typeof(string))
                .SetValue(S3FileProviderConfigurationNames.SecretAccessKey, typeof(string))
                .SetValue(S3FileProviderConfigurationNames.ForcePathStyle, typeof(bool))
                .SetValue(S3FileProviderConfigurationNames.UseChunkEncoding, typeof(bool))
                .SetValue(S3FileProviderConfigurationNames.Protocol, typeof(int))
                .SetValue(S3FileProviderConfigurationNames.VendorType, typeof(int))
                .SetValue(S3FileProviderConfigurationNames.EnableSlice, typeof(bool))
                .SetValue(S3FileProviderConfigurationNames.SliceSize, typeof(int))
                .SetValue(S3FileProviderConfigurationNames.SignatureVersion, typeof(string))
                .SetValue(S3FileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool))
                .SetValue(S3FileProviderConfigurationNames.MaxClient, typeof(int));

            return configuration;
        }
    }
}

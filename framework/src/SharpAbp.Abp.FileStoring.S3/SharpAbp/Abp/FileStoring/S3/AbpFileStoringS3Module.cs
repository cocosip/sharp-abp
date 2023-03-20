using SharpAbp.Abp.FileStoring.S3.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.S3
{
    [DependsOn(
        typeof(AbpFileStoringModule)
        )]
    public class AbpFileStoringS3Module : AbpModule
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
                options.FileSets.AddEmbedded<AbpFileStoringS3Module>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringS3Resource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/S3/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringS3", typeof(FileStoringS3Resource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                S3FileProviderConfigurationNames.ProviderName,
                typeof(FileStoringS3Resource));

            configuration.DefaultNamingNormalizers.TryAdd<S3FileNamingNormalizer>();
            configuration
                .AddItem(S3FileProviderConfigurationNames.BucketName, typeof(string), "bucket1")
                .AddItem(S3FileProviderConfigurationNames.ServerUrl, typeof(string), "http://192.168.0.100:9005")
                .AddItem(S3FileProviderConfigurationNames.AccessKeyId, typeof(string), "")
                .AddItem(S3FileProviderConfigurationNames.SecretAccessKey, typeof(string), "")
                .AddItem(S3FileProviderConfigurationNames.ForcePathStyle, typeof(bool), "false")
                .AddItem(S3FileProviderConfigurationNames.UseChunkEncoding, typeof(bool), "false")
                .AddItem(S3FileProviderConfigurationNames.Protocol, typeof(int), "1-http,2-https")
                .AddItem(S3FileProviderConfigurationNames.SignatureVersion, typeof(string), "v2.0")
                .AddItem(S3FileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool), "false");
            return configuration;
        }
    }
}

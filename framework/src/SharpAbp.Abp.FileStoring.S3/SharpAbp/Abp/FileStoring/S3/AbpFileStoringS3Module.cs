using SharpAbp.Abp.FileStoring.S3.Localization;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
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

            return Task.CompletedTask;
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
                .AddItem(S3FileProviderConfigurationNames.Protocol, typeof(int), "0-https,1-http")
                .AddItem(S3FileProviderConfigurationNames.SignatureVersion, typeof(string), "2 | 4")
                .AddItem(S3FileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool), "false");
            return configuration;
        }
    }
}

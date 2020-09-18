using SharpAbp.Abp.AmazonS3.KS3;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring.S3
{
    [DependsOn(
      typeof(AbpFileStoringModule),
      typeof(AbpAmazonS3KS3Module)
      )]
    public class AbpFileStoringS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
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

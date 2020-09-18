using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring.Minio
{
    [DependsOn(typeof(AbpFileStoringModule))]
    public class AbpFileStoringMinioModule : AbpModule
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
            var configuration = new FileProviderConfiguration(typeof(MinioFileProvider));
            configuration.DefaultNamingNormalizers.TryAdd<MinioFileNamingNormalizer>();
            configuration
                .SetProperty(MinioFileProviderConfigurationNames.BucketName, typeof(string))
                .SetProperty(MinioFileProviderConfigurationNames.EndPoint, typeof(string))
                .SetProperty(MinioFileProviderConfigurationNames.AccessKey, typeof(string))
                .SetProperty(MinioFileProviderConfigurationNames.SecretKey, typeof(string))
                .SetProperty(MinioFileProviderConfigurationNames.WithSSL, typeof(bool))
                .SetProperty(MinioFileProviderConfigurationNames.CreateBucketIfNotExists, typeof(bool))
                ;

            return configuration;
        }
    }
}

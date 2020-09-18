using SharpAbp.Abp.FileStoring.Aliyun.Localization;
using Volo.Abp.Caching;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpCachingModule),
        typeof(AbpValidationModule)
    )]
    public class AbpFileStoringAliyunModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpFileStoringAliyunModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringAliyunResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Aliyun/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringAliyun", typeof(FileStoringAliyunResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(typeof(AliyunFileProvider));
            configuration.DefaultNamingNormalizers.TryAdd<AliyunFileNamingNormalizer>();
            configuration
                .SetProperty(AliyunFileProviderConfigurationNames.RegionId, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.Endpoint, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.BucketName, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.AccessKeyId, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.AccessKeySecret, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.UseSecurityTokenService, typeof(int))
                .SetProperty(AliyunFileProviderConfigurationNames.RoleArn, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.RoleSessionName, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.DurationSeconds, typeof(int))
                .SetProperty(AliyunFileProviderConfigurationNames.Policy, typeof(string))
                .SetProperty(AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool))
                .SetProperty(AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, typeof(string));
            
            return configuration;
        }
    }
}

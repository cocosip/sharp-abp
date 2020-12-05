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
            var configuration = new FileProviderConfiguration(AliyunFileProviderConfigurationNames.ProviderName);
            configuration.DefaultNamingNormalizers.TryAdd<AliyunFileNamingNormalizer>();
            configuration
                .SetValue(AliyunFileProviderConfigurationNames.RegionId, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.Endpoint, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.BucketName, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.AccessKeyId, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.AccessKeySecret, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.UseSecurityTokenService, typeof(bool))
                .SetValue(AliyunFileProviderConfigurationNames.RoleArn, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.RoleSessionName, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.DurationSeconds, typeof(int))
                .SetValue(AliyunFileProviderConfigurationNames.Policy, typeof(string))
                .SetValue(AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool))
                .SetValue(AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, typeof(string));

            return configuration;
        }
    }
}

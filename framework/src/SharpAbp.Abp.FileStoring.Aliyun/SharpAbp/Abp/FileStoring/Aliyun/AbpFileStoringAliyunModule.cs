using SharpAbp.Abp.FileStoring.Aliyun.Localization;
using Volo.Abp.Caching;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpCachingModule)
        )]
    public class AbpFileStoringAliyunModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(options =>
            {
                var configuration = GetFileProviderConfiguration();
                options.Providers.TryAdd(configuration);
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
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Aliyun/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringAliyun", typeof(FileStoringAliyunResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(AliyunFileProviderConfigurationNames.ProviderName, typeof(FileStoringAliyunResource));
            configuration.DefaultNamingNormalizers.TryAdd<AliyunFileNamingNormalizer>();
            configuration
                .SetValueType(AliyunFileProviderConfigurationNames.RegionId, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.Endpoint, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.BucketName, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.AccessKeyId, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.AccessKeySecret, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.UseSecurityTokenService, typeof(bool))
                .SetValueType(AliyunFileProviderConfigurationNames.RoleArn, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.RoleSessionName, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.DurationSeconds, typeof(int))
                .SetValueType(AliyunFileProviderConfigurationNames.Policy, typeof(string))
                .SetValueType(AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool))
                .SetValueType(AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, typeof(string));

            return configuration;
        }
    }
}

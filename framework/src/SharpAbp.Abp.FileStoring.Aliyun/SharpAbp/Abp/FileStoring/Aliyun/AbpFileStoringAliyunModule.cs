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
            var configuration = new FileProviderConfiguration(
                AliyunFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringAliyunResource));

            configuration.DefaultNamingNormalizers.TryAdd<AliyunFileNamingNormalizer>();
            configuration
                .AddItem(AliyunFileProviderConfigurationNames.RegionId, typeof(string), "oss-cn-hangzhou")
                .AddItem(AliyunFileProviderConfigurationNames.Endpoint, typeof(string), "https://oss-cn-hangzhou.aliyuncs.com")
                .AddItem(AliyunFileProviderConfigurationNames.BucketName, typeof(string), "bucket1")
                .AddItem(AliyunFileProviderConfigurationNames.AccessKeyId, typeof(string), "ak")
                .AddItem(AliyunFileProviderConfigurationNames.AccessKeySecret, typeof(string), "sk")
                .AddItem(AliyunFileProviderConfigurationNames.UseSecurityTokenService, typeof(bool), "false")
                .AddItem(AliyunFileProviderConfigurationNames.RoleArn, typeof(string), "")
                .AddItem(AliyunFileProviderConfigurationNames.RoleSessionName, typeof(string), "")
                .AddItem(AliyunFileProviderConfigurationNames.DurationSeconds, typeof(int), "3600")
                .AddItem(AliyunFileProviderConfigurationNames.Policy, typeof(string), "")
                .AddItem(AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool), "false")
                .AddItem(AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, typeof(string), "OssCacheKey");

            return configuration;
        }
    }
}

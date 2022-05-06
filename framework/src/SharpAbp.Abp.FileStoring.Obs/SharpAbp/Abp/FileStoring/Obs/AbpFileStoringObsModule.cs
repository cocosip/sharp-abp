using SharpAbp.Abp.FileStoring.Obs.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.Obs
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpTimingModule)
        )]
    public class AbpFileStoringObsModule : AbpModule
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
                options.FileSets.AddEmbedded<AbpFileStoringObsModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringObsResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/Obs/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringObs", typeof(FileStoringObsResource));
            });
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                ObsFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringObsResource));

            configuration.DefaultNamingNormalizers.TryAdd<ObsFileNamingNormalizer>();
            configuration
                //.SetValueType(ObsFileProviderConfigurationNames.RegionId, typeof(string), "")
                .SetValueType(ObsFileProviderConfigurationNames.Endpoint, typeof(string), "https://obs.cn-east-3.myhuaweicloud.com")
                .SetValueType(ObsFileProviderConfigurationNames.BucketName, typeof(string), "bucket1")
                .SetValueType(ObsFileProviderConfigurationNames.AccessKeyId, typeof(string), "")
                .SetValueType(ObsFileProviderConfigurationNames.AccessKeySecret, typeof(string), "")
                .SetValueType(ObsFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool), "false");

            return configuration;
        }
    }
}

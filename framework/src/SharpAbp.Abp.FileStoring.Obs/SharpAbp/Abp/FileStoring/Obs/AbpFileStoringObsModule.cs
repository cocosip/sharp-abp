using SharpAbp.Abp.FileStoring.Obs.Localization;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
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
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<AbpFileStoringAbstractionsOptions>(options =>
            {
                var configuration = GetFileProviderConfiguration();
                options.Providers.TryAdd(configuration);
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

            return Task.CompletedTask;
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                ObsFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringObsResource));

            configuration.DefaultNamingNormalizers.TryAdd<ObsFileNamingNormalizer>();
            configuration
                //.AddItem(ObsFileProviderConfigurationNames.RegionId, typeof(string), "")
                .AddItem(ObsFileProviderConfigurationNames.Endpoint, typeof(string), "https://obs.cn-east-3.myhuaweicloud.com")
                .AddItem(ObsFileProviderConfigurationNames.BucketName, typeof(string), "bucket1")
                .AddItem(ObsFileProviderConfigurationNames.AccessKeyId, typeof(string), "")
                .AddItem(ObsFileProviderConfigurationNames.AccessKeySecret, typeof(string), "")
                .AddItem(ObsFileProviderConfigurationNames.CreateContainerIfNotExists, typeof(bool), "false");

            return configuration;
        }
    }
}

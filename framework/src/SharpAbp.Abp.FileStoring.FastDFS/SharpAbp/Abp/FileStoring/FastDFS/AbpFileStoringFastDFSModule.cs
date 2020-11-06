using FastDFSCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.FastDFS.DotNetty;
using SharpAbp.Abp.FileStoring.FastDFS.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    [DependsOn(
      typeof(AbpFileStoringModule),
      typeof(AbpFastDFSDotNettyModule),
      typeof(AbpValidationModule)
      )]
    public class AbpFileStoringFastDFSModule : AbpModule
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
            context.Services.Replace(ServiceDescriptor.Singleton<IClusterSelector, FileConfigurationClusterSelector>());

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpFileStoringFastDFSModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringFastDFSResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/FastDFS/Localization");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringFastDFS", typeof(FileStoringFastDFSResource));
            });
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(FastDFSFileProviderConfigurationNames.ProviderName);
            configuration.DefaultNamingNormalizers.TryAdd<FastDFSFileNamingNormalizer>();
            configuration
                .SetProperty(FastDFSFileProviderConfigurationNames.ClusterName, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.GroupName, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.HttpServer, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, typeof(bool))
                .SetProperty(FastDFSFileProviderConfigurationNames.Trackers, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, typeof(bool))
                .SetProperty(FastDFSFileProviderConfigurationNames.SecretKey, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.ConnectionTimeout, typeof(int))
                .SetProperty(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, typeof(int))
                .SetProperty(FastDFSFileProviderConfigurationNames.Charset, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, typeof(int))
                .SetProperty(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, typeof(int))
                .SetProperty(FastDFSFileProviderConfigurationNames.TrackerMaxConnection, typeof(int))
                .SetProperty(FastDFSFileProviderConfigurationNames.StorageMaxConnection, typeof(int));
            return configuration;
        }
    }
}

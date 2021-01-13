using FastDFSCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.FastDFS.DotNetty;
using SharpAbp.Abp.FileStoring.FastDFS.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpFastDFSDotNettyModule)
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
                .SetValue(FastDFSFileProviderConfigurationNames.ClusterName, typeof(string))
                .SetValue(FastDFSFileProviderConfigurationNames.GroupName, typeof(string))
                .SetValue(FastDFSFileProviderConfigurationNames.HttpServer, typeof(string))
                .SetValue(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, typeof(bool))
                .SetValue(FastDFSFileProviderConfigurationNames.Trackers, typeof(string))
                .SetValue(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, typeof(bool))
                .SetValue(FastDFSFileProviderConfigurationNames.SecretKey, typeof(string))
                .SetValue(FastDFSFileProviderConfigurationNames.ConnectionTimeout, typeof(int))
                .SetValue(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, typeof(int))
                .SetValue(FastDFSFileProviderConfigurationNames.Charset, typeof(string))
                .SetValue(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, typeof(int))
                .SetValue(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, typeof(int))
                .SetValue(FastDFSFileProviderConfigurationNames.TrackerMaxConnection, typeof(int))
                .SetValue(FastDFSFileProviderConfigurationNames.StorageMaxConnection, typeof(int));
            return configuration;
        }
    }
}

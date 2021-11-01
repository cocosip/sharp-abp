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
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/FastDFS/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringFastDFS", typeof(FileStoringFastDFSResource));
            });
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(FastDFSFileProviderConfigurationNames.ProviderName,typeof(FileStoringFastDFSResource));
            configuration.DefaultNamingNormalizers.TryAdd<FastDFSFileNamingNormalizer>();
            configuration
                .SetValueType(FastDFSFileProviderConfigurationNames.ClusterName, typeof(string))
                .SetValueType(FastDFSFileProviderConfigurationNames.GroupName, typeof(string))
                .SetValueType(FastDFSFileProviderConfigurationNames.HttpServer, typeof(string))
                .SetValueType(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, typeof(bool))
                .SetValueType(FastDFSFileProviderConfigurationNames.Trackers, typeof(string))
                .SetValueType(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, typeof(bool))
                .SetValueType(FastDFSFileProviderConfigurationNames.SecretKey, typeof(string))
                .SetValueType(FastDFSFileProviderConfigurationNames.ConnectionTimeout, typeof(int))
                .SetValueType(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, typeof(int))
                .SetValueType(FastDFSFileProviderConfigurationNames.Charset, typeof(string))
                .SetValueType(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, typeof(int))
                .SetValueType(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, typeof(int))
                .SetValueType(FastDFSFileProviderConfigurationNames.TrackerMaxConnection, typeof(int))
                .SetValueType(FastDFSFileProviderConfigurationNames.StorageMaxConnection, typeof(int));
            return configuration;
        }
    }
}

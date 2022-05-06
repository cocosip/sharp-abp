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
            var configuration = new FileProviderConfiguration(
                FastDFSFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringFastDFSResource));

            configuration.DefaultNamingNormalizers.TryAdd<FastDFSFileNamingNormalizer>();
            configuration
                .SetValueType(FastDFSFileProviderConfigurationNames.ClusterName, typeof(string), "default1")
                .SetValueType(FastDFSFileProviderConfigurationNames.GroupName, typeof(string), "group1")
                .SetValueType(FastDFSFileProviderConfigurationNames.HttpServer, typeof(string), "http://192.168.0.100:8080")
                .SetValueType(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, typeof(bool), "true")
                .SetValueType(FastDFSFileProviderConfigurationNames.Trackers, typeof(string), "192.168.0.100:23000,192.168.0.101:23000")
                .SetValueType(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, typeof(bool), "true")
                .SetValueType(FastDFSFileProviderConfigurationNames.SecretKey, typeof(string), "abc123456789")
                .SetValueType(FastDFSFileProviderConfigurationNames.ConnectionTimeout, typeof(int), "3600")
                .SetValueType(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, typeof(int), "300")
                .SetValueType(FastDFSFileProviderConfigurationNames.Charset, typeof(string), "utf-8")
                .SetValueType(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, typeof(int), "3")
                .SetValueType(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, typeof(int), "10")
                .SetValueType(FastDFSFileProviderConfigurationNames.TrackerMaxConnection, typeof(int), "10")
                .SetValueType(FastDFSFileProviderConfigurationNames.StorageMaxConnection, typeof(int), "20");
            return configuration;
        }
    }
}

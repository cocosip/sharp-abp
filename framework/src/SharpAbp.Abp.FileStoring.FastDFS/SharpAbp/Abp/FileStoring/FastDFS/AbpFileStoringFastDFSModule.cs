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
            PreConfigure<AbpFileStoringAbstractionsOptions>(c =>
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
                .AddItem(FastDFSFileProviderConfigurationNames.ClusterName, typeof(string), "default1")
                .AddItem(FastDFSFileProviderConfigurationNames.GroupName, typeof(string), "group1")
                .AddItem(FastDFSFileProviderConfigurationNames.HttpServer, typeof(string), "http://192.168.0.100:8080")
                .AddItem(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, typeof(bool), "true")
                .AddItem(FastDFSFileProviderConfigurationNames.Trackers, typeof(string), "192.168.0.100:23000,192.168.0.101:23000")
                .AddItem(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, typeof(bool), "true")
                .AddItem(FastDFSFileProviderConfigurationNames.SecretKey, typeof(string), "abc123456789")
                .AddItem(FastDFSFileProviderConfigurationNames.ConnectionTimeout, typeof(int), "3600")
                .AddItem(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, typeof(int), "300")
                .AddItem(FastDFSFileProviderConfigurationNames.Charset, typeof(string), "utf-8")
                .AddItem(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, typeof(int), "3")
                .AddItem(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, typeof(int), "10")
                .AddItem(FastDFSFileProviderConfigurationNames.TrackerMaxConnection, typeof(int), "10")
                .AddItem(FastDFSFileProviderConfigurationNames.StorageMaxConnection, typeof(int), "20");
            return configuration;
        }
    }
}

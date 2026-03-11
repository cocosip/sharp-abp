using SharpAbp.Abp.FastDFS;
using SharpAbp.Abp.FileStoring.FastDFS.Localization;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpFastDFSModule)
        )]
    public class AbpFileStoringFastDFSModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<AbpFileStoringAbstractionsOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
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
            return Task.CompletedTask;
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                FastDFSFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringFastDFSResource));

            configuration.DefaultNamingNormalizers.TryAdd<FastDFSFileNamingNormalizer>();
            configuration
                .AddItem(FastDFSFileProviderConfigurationNames.ClusterName, typeof(string), "default")
                .AddItem(FastDFSFileProviderConfigurationNames.GroupName, typeof(string), "group1")
                .AddItem(FastDFSFileProviderConfigurationNames.HttpServer, typeof(string), "http://192.168.0.100:8080")
                .AddItem(FastDFSFileProviderConfigurationNames.Trackers, typeof(string), "192.168.0.100:22122,192.168.0.101:22122")
                .AddItem(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, typeof(bool), "false")
                .AddItem(FastDFSFileProviderConfigurationNames.SecretKey, typeof(string), "")
                .AddItem(FastDFSFileProviderConfigurationNames.DefaultTokenExpireSeconds, typeof(int), "3600")
                .AddItem(FastDFSFileProviderConfigurationNames.Charset, typeof(string), "UTF-8")
                .AddItem(FastDFSFileProviderConfigurationNames.NetworkTimeout, typeof(int), "30")
                .AddItem(FastDFSFileProviderConfigurationNames.MaxConnectionPerServer, typeof(int), "50")
                .AddItem(FastDFSFileProviderConfigurationNames.MinConnectionPerServer, typeof(int), "5")
                .AddItem(FastDFSFileProviderConfigurationNames.ConnectionIdleTimeout, typeof(int), "300")
                .AddItem(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, typeof(int), "3600")
                .AddItem(FastDFSFileProviderConfigurationNames.ConnectionTimeout, typeof(int), "30000")
                .AddItem(FastDFSFileProviderConfigurationNames.SendTimeout, typeof(int), "30000")
                .AddItem(FastDFSFileProviderConfigurationNames.ReceiveTimeout, typeof(int), "30000");
            return configuration;
        }
    }
}

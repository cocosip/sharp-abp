using FastDFSCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.FastDFS.DotNetty;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
      typeof(AbpFileStoringModule),
      typeof(AbpFastDFSDotNettyModule)
      )]
    public class AbpFileStoringFastDFSModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Singleton<IClusterSelector, FileConfigurationClusterSelector>());

            Configure<AbpFileStoringOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(typeof(FastDFSFileProvider));
            configuration.DefaultNamingNormalizers.TryAdd<FastDFSFileNamingNormalizer>();
            configuration
                .SetProperty(FastDFSFileProviderConfigurationNames.ClusterName, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.GroupName, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.HttpServer, typeof(string))
                .SetProperty(FastDFSFileProviderConfigurationNames.Trackers, typeof(string))
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

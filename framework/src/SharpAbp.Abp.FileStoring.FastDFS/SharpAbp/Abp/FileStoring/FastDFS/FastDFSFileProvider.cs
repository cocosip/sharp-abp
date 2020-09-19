using FastDFSCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class FastDFSFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IFastDFSFileNameCalculator FastDFSFileNameCalculator { get; }
        protected IFastDFSFileProviderConfigurationFactory ConfigurationFactory { get; }
        protected IFastDFSClient Client { get; }

        public FastDFSFileProvider(ILogger<FastDFSFileProvider> logger, IFastDFSFileNameCalculator fastDFSFileNameCalculator, IFastDFSFileProviderConfigurationFactory configurationFactory, IFastDFSClient client)
        {
            Logger = logger;
            FastDFSFileNameCalculator = fastDFSFileNameCalculator;
            ConfigurationFactory = configurationFactory;
            Client = client;
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var containerName = GetContainerName(configuration, args);
            var storageNode = await Client.GetStorageNodeAsync(containerName, configuration.ClusterName);
            var fileId = await Client.UploadFileAsync(storageNode, args.FileStream, args.FileExt, configuration.ClusterName);
            return fileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FastDFSFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);
            return await Client.RemoveFileAsync(containerName, fileId, configuration.ClusterName);
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FastDFSFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);

            var storageNode = await Client.GetStorageNodeAsync(containerName, configuration.ClusterName);
            var fileInfo = await Client.GetFileInfo(storageNode, fileId, configuration.ClusterName);
            return fileInfo != null;
        }


        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FastDFSFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);
            var storageNode = await Client.GetStorageNodeAsync(containerName, configuration.ClusterName);
            try
            {
                await Client.DownloadFileEx(storageNode, fileId, args.Path, configuration.ClusterName);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogLevel.Error);
                return false;
            }
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FastDFSFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);
            var storageNode = await Client.GetStorageNodeAsync(containerName, configuration.ClusterName);

            try
            {
                var content = await Client.DownloadFileAsync(storageNode, fileId, configuration.ClusterName);
                return new MemoryStream(content);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogLevel.Error);
                return null;
            }
        }


        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpSupport)
            {
                return Task.FromResult("");
            }

            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FastDFSFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);

            var accessUrl = BuildAccessUrl(configuration, containerName, fileId);
            return Task.FromResult(accessUrl);
        }


        private static string GetContainerName(FastDFSFileProviderConfiguration configuration, FileProviderArgs args)
        {
            return configuration.GroupName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : configuration.GroupName;
        }


        protected virtual string BuildAccessUrl(FastDFSFileProviderConfiguration configuration, string containerName, string fileId)
        {
            var accessUrl = $"{configuration.HttpServer.TrimEnd('/')}/{containerName}/{fileId}";
            return accessUrl;
        }

    }
}

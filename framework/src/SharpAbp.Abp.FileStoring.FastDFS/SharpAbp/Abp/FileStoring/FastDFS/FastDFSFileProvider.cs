using DotNetty.Common.Utilities;
using FastDFSCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class FastDFSFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IFastDFSFileNameCalculator FastDFSFileNameCalculator { get; }
        protected IFastDFSFileProviderConfigurationFactory ConfigurationFactory { get; }
        protected IFastDFSClient Client { get; }

        public FastDFSFileProvider(ILogger<FastDFSFileProvider> logger, IClock clock, IFastDFSFileNameCalculator fastDFSFileNameCalculator, IFastDFSFileProviderConfigurationFactory configurationFactory, IFastDFSClient client)
        {
            Logger = logger;
            Clock = clock;
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
            if (configuration.AntiStealCheckToken)
            {
                return $"{configuration.HttpServer.TrimEnd('/')}/{containerName}/{fileId}";
            }
            else
            {
                var timestamp = ToInt32(Clock.Now);
                var token = Client.GetToken(fileId, timestamp, configuration.ClusterName);
                return $"{configuration.HttpServer.TrimEnd('/')}/{containerName}/{fileId}?token={token}&ts={timestamp}";
            }
        }


        /// <summary>Convert time to int32 timestamp(from 1970-01-01 00:00:00)
        /// </summary>
        /// <param name="datetime">time</param>
        /// <returns></returns>
        protected virtual int ToInt32(DateTime datetime)
        {
            var timeSpan = datetime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32(timeSpan.TotalSeconds);
        }

    }
}

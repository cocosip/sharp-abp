using FastDFS.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using FastDFSCore = FastDFS;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    [ExposeKeyedService<IFileProvider>(FastDFSFileProviderConfigurationNames.ProviderName)]
    public class FastDFSFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IFastDFSFileNameCalculator FileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        protected IFastDFSFileProviderConfigurationFactory ConfigurationFactory { get; }
        public FastDFSFileProvider(
            ILogger<FastDFSFileProvider> logger,
            IClock clock,
            IFastDFSFileNameCalculator fileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService,
            IFastDFSFileProviderConfigurationFactory configurationFactory)
        {
            Logger = logger;
            Clock = clock;
            FileNameCalculator = fileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
            ConfigurationFactory = configurationFactory;
        }

        public override string Provider => FastDFSFileProviderConfigurationNames.ProviderName;


        protected virtual IFastDFSClient GetClient(FastDFSFileProviderConfiguration configuration)
        {
            var client = FastDFSClientBuilder.CreateClient(new FastDFSCore.Client.Configuration.FastDFSConfiguration()
            {
                TrackerServers = [.. configuration.Trackers.ToTrackers()],
                NetworkTimeout = configuration.ConnectionTimeout,
                DefaultGroupName = configuration.GroupName,
                Charset = configuration.Charset,
                HttpConfig = new FastDFSCore.Client.Configuration.HttpConfiguration()
                {
                    ServerUrls = new Dictionary<string, string>()
                    {
                        { configuration.ClusterName, configuration.HttpServer }
                    },
                    SecretKey = configuration.SecretKey,
                    AntiStealTokenEnabled = configuration.AntiStealCheckToken,
                },

            }, configuration.ClusterName);
            return client;
        }


        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            ConfigurationFactory.AddIfNotContains(configuration);
            var client = GetClient(configuration);
            var fileId = await client.UploadAsync(configuration.GroupName, args.FileStream!, args.FileExt!, args.CancellationToken);
            return fileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            try
            {
                var configuration = args.Configuration.GetFastDFSConfiguration();

                ConfigurationFactory.AddIfNotContains(configuration);

                var fileId = FileNameCalculator.Calculate(args);
                var client = GetClient(configuration);

                await client.DeleteAsync(fileId, args.CancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to delete file from FastDFS. Args: {Args}", args);
                return false;
            }
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var client = GetClient(configuration);
            return await client.FileExistsAsync(fileId, args.CancellationToken);

        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var client = GetClient(configuration);

            try
            {
                await client.DownloadFileAsync(fileId, args.Path, args.CancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to download file '{FileId}' from FastDFS. Container: {ContainerName}, Cluster: {ClusterName}",
                    fileId, args.ContainerName, configuration.ClusterName);
                return false;
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);
            var client = GetClient(configuration);

            try
            {
                var content = await client.DownloadAsync(fileId, args.CancellationToken);
                var ms = new MemoryStream(content);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get file '{FileId}' from FastDFS. Container: {ContainerName}, Cluster: {ClusterName}",
                    fileId, containerName, configuration.ClusterName);
                return null;
            }
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var configuration = args.Configuration.GetFastDFSConfiguration();

            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);

            var accessUrl = await BuildAccessUrl(configuration, containerName, fileId);
            return accessUrl;
        }

        private string GetContainerName(FastDFSFileProviderConfiguration configuration, FileProviderArgs args)
        {
            return configuration.GroupName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.GroupName);
        }

        protected virtual async Task<string> BuildAccessUrl(
            FastDFSFileProviderConfiguration configuration,
            string containerName,
            string fileId)
        {
            if (configuration.AntiStealCheckToken)
            {
                return $"{configuration.HttpServer.EnsureEndsWith('/')}/{containerName}/{fileId}";
            }
            else
            {
                var timestamp = ToInt32(Clock.Now);
                var client = GetClient(configuration);
                var url = await client.GetFileUrlWithTokenAsync(fileId, timestamp);
                return url;
            }
        }

        /// <summary>
        /// Convert time to int32 timestamp (from 1970-01-01 00:00:00)
        /// </summary>
        /// <param name="datetime">Time to convert</param>
        /// <returns>Timestamp as int32</returns>
        protected virtual int ToInt32(DateTime datetime)
        {
            var timeSpan = datetime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32(timeSpan.TotalSeconds);
        }

    }
}

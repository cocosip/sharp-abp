using FastDFS.Client;
using FastDFS.Client.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    [ExposeKeyedService<IFileProvider>(FastDFSFileProviderConfigurationNames.ProviderName)]
    public class FastDFSFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IFastDFSFileNameCalculator FileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        protected IFastDFSFileProviderConfigurationFactory ConfigurationFactory { get; }
        protected IFastDFSClientFactory ClientFactory { get; }

        public FastDFSFileProvider(
            ILogger<FastDFSFileProvider> logger,
            IFastDFSFileNameCalculator fileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService,
            IFastDFSFileProviderConfigurationFactory configurationFactory,
            IFastDFSClientFactory clientFactory)
        {
            Logger = logger;
            FileNameCalculator = fileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
            ConfigurationFactory = configurationFactory;
            ClientFactory = clientFactory;
        }

        public override string Provider => FastDFSFileProviderConfigurationNames.ProviderName;

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            ConfigurationFactory.AddIfNotContains(configuration);

            var client = ClientFactory.GetClient(configuration.ClusterName);
            var groupName = GetGroupName(configuration, args);
            return await client.UploadAsync(groupName, args.FileStream!, args.FileExt!);
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var client = ClientFactory.GetClient(configuration.ClusterName);

            try
            {
                await client.DeleteAsync(fileId);
                return true;
            }
            catch (FastDFSException ex)
            {
                Logger.LogWarning(ex, "Failed to delete file '{FileId}' from FastDFS. Cluster: {ClusterName}",
                    fileId, configuration.ClusterName);
                return false;
            }
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var client = ClientFactory.GetClient(configuration.ClusterName);
            return await client.FileExistsAsync(fileId);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var client = ClientFactory.GetClient(configuration.ClusterName);

            try
            {
                return await TryWriteToFileAsync(args.Path, async (output, token) =>
                {
                    await client.DownloadAsync(fileId, output, token);
                    return true;
                }, args.CancellationToken);
            }
            catch (FastDFSException ex)
            {
                Logger.LogWarning(ex, "File not found or failed to download '{FileId}' from FastDFS. Cluster: {ClusterName}",
                    fileId, configuration.ClusterName);
                return false;
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            ConfigurationFactory.AddIfNotContains(configuration);

            var fileId = FileNameCalculator.Calculate(args);
            var client = ClientFactory.GetClient(configuration.ClusterName);

            try
            {
                var ms = new MemoryStream();
                await client.DownloadAsync(fileId, ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
            catch (FastDFSException ex)
            {
                Logger.LogWarning(ex, "File not found or failed to get '{FileId}' from FastDFS. Cluster: {ClusterName}",
                    fileId, configuration.ClusterName);
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
            var client = ClientFactory.GetClient(configuration.ClusterName);

            if (args.CheckFileExist && !await client.FileExistsAsync(fileId, args.CancellationToken))
            {
                return string.Empty;
            }

            if (args.Expires.HasValue)
            {
                throw new NotSupportedException("FastDFS access URLs cannot enforce an explicit expiration because the server controls the token lifetime.");
            }

            return configuration.AntiStealCheckToken
                ? await client.GetFileUrlWithTokenAsync(fileId, configuration.DefaultTokenExpireSeconds, cancellationToken: args.CancellationToken)
                : await client.GetFileUrlAsync(fileId, cancellationToken: args.CancellationToken);
        }

        private string GetGroupName(FastDFSFileProviderConfiguration configuration, FileProviderArgs args)
        {
            return configuration.GroupName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.GroupName);
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Azure
{
    [ExposeKeyedService<IFileProvider>(AzureFileProviderConfigurationNames.ProviderName)]
    public class AzureFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IAzureFileNameCalculator AzureFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public AzureFileProvider(
            ILogger<AzureFileProvider> logger,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IPoolOrchestrator poolOrchestrator,
            IAzureFileNameCalculator azureFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            Options = options.Value;
            PoolOrchestrator = poolOrchestrator;
            AzureFileNameCalculator = azureFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => AzureFileProviderConfigurationNames.ProviderName;

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetAzureConfiguration();

            if (!args.OverrideExisting && await FileExistsAsync(args, fileName))
            {
                throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{GetContainerName(args)}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateContainerIfNotExists)
            {
                await CreateContainerIfNotExists(args);
            }

            await GetBlobClient(args, fileName).UploadAsync(
                args.FileStream,
                args.OverrideExisting,
                args.CancellationToken);
            return args.FileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);

            if (await FileExistsAsync(args, fileName))
            {
                return await GetBlobClient(args, fileName).DeleteIfExistsAsync();
            }

            Logger.LogWarning("File not found in Azure Blob Storage when deleting. Container: {ContainerName}, FileName: {FileName}, FileId: {FileId}", GetContainerName(args), fileName, args.FileId);
            return false;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);

            return await FileExistsAsync(args, fileName);
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);
            if (!await FileExistsAsync(args, fileName))
            {
                Logger.LogWarning("File not found in Azure Blob Storage. Container: {ContainerName}, FileName: {FileName}, FileId: {FileId}", GetContainerName(args), fileName, args.FileId);
                return null;
            }

            var blobClient = GetBlobClient(args, fileName);
            var download = await blobClient.DownloadAsync(args.CancellationToken);
            using var content = download.Value.Content;
            return await TryCopyToMemoryStreamAsync(content, args.CancellationToken);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);
            if (!await FileExistsAsync(args, fileName))
            {
                Logger.LogWarning("File not found in Azure Blob Storage. Container: {ContainerName}, FileName: {FileName}, FileId: {FileId}", GetContainerName(args), fileName, args.FileId);
                return false;
            }
            var blobClient = GetBlobClient(args, fileName);
            var download = await blobClient.DownloadToAsync(args.Path, args.CancellationToken);
            return !download.IsError;
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            return Task.FromResult(string.Empty);
        }

        protected virtual BlobClient GetBlobClient(FileProviderArgs args, string fileName)
        {
            var blobContainerClient = GetBlobContainerClient(args);
            return blobContainerClient.GetBlobClient(fileName);
        }

        protected virtual BlobContainerClient GetBlobContainerClient(FileProviderArgs args)
        {
            var pool = GetBlobContainerClientPool(args);
            var blobContainerClient = pool.Get();
            pool.Return(blobContainerClient);
            return blobContainerClient;
        }

        protected virtual IObjectPool<BlobContainerClient> GetBlobContainerClientPool(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAzureConfiguration();
            var containerName = GetContainerName(args);
            var poolName = NormalizePoolName(configuration.ConnectionString, containerName);

            return PoolOrchestrator.GetObjectPool<BlobContainerClient, AzureBlobContainerClientPolicy>(
                poolName,
                () => new AzureBlobContainerClientPolicy(configuration.ConnectionString, containerName),
                Options.DefaultClientMaximumRetained);
        }

        protected virtual string NormalizePoolName(string connectionString, string containerName)
        {
            var v = $"{connectionString}-{containerName}";
            using var sha1 = SHA1.Create();
            var hashBuffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(v));
            var hash = hashBuffer.Aggregate("", (current, b) => current + b.ToString("X2"));
            return $"FileStoring-{AzureFileProviderConfigurationNames.ProviderName}-{hash}";
        }

        protected virtual async Task CreateContainerIfNotExists(FileProviderArgs args)
        {
            var blobContainerClient = GetBlobContainerClient(args);
            await blobContainerClient.CreateIfNotExistsAsync();
        }

        private async Task<bool> FileExistsAsync(FileProviderArgs args, string fileName)
        {
            return await ContainerExistsAsync(GetBlobContainerClient(args)) &&
                   (await GetBlobClient(args, fileName).ExistsAsync()).Value;
        }

        private string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAzureConfiguration();
            return configuration.ContainerName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.ContainerName);
        }

        private async Task<bool> ContainerExistsAsync(BlobContainerClient blobContainerClient)
        {
            return (await blobContainerClient.ExistsAsync()).Value;
        }
    }
}

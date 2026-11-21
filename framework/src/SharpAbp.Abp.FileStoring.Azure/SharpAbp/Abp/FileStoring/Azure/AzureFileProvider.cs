using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Azure
{
    [ExposeKeyedService<IFileProvider>(AzureFileProviderConfigurationNames.ProviderName)]
    public class AzureFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected FileProviderClientCache<BlobServiceClient> ClientCache { get; }
        protected IAzureFileNameCalculator AzureFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        protected IClock Clock { get; }

        public AzureFileProvider(
            ILogger<AzureFileProvider> logger,
            FileProviderClientCache<BlobServiceClient> clientCache,
            IAzureFileNameCalculator azureFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService,
            IClock clock)
        {
            Logger = logger;
            ClientCache = clientCache;
            AzureFileNameCalculator = azureFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
            Clock = clock;
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
            return await TryWriteToFileAsync(args.Path, async (output, token) =>
            {
                var download = await blobClient.DownloadToAsync(output, token);
                return !download.IsError;
            }, args.CancellationToken);
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var fileName = AzureFileNameCalculator.Calculate(args);
            var blobClient = GetBlobClient(args, fileName);

            if (args.CheckFileExist && !(await blobClient.ExistsAsync(args.CancellationToken)).Value)
            {
                return string.Empty;
            }

            if (!args.Expires.HasValue)
            {
                return blobClient.Uri.AbsoluteUri;
            }

            var expiresOn = args.Expires.Value > Clock.Now
                ? args.Expires.Value
                : Clock.Now.AddMinutes(10);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = GetContainerName(args),
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = expiresOn
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
        }

        protected virtual BlobClient GetBlobClient(FileProviderArgs args, string fileName)
        {
            var blobContainerClient = GetBlobContainerClient(args);
            return blobContainerClient.GetBlobClient(fileName);
        }

        protected virtual BlobContainerClient GetBlobContainerClient(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAzureConfiguration();
            var blobServiceClient = ClientCache.GetOrAdd(
                BuildClientKey(configuration),
                () => new BlobServiceClient(configuration.ConnectionString));

            return blobServiceClient.GetBlobContainerClient(GetContainerName(args));
        }

        protected virtual string BuildClientKey(AzureFileProviderConfiguration configuration)
        {
            return FileProviderClientKey.Build(
                Provider,
                ("ConnectionString", configuration.ConnectionString));
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

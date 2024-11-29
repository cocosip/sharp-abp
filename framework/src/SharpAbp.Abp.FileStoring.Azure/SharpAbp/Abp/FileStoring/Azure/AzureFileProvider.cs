﻿using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Azure
{
    [ExposeKeyedService<IFileProvider>(AzureFileProviderConfigurationNames.ProviderName)]
    public class AzureFileProvider : FileProviderBase, ITransientDependency
    {
        protected IAzureFileNameCalculator AzureFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        public AzureFileProvider(
            IAzureFileNameCalculator azureFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
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

            await GetBlobClient(args, fileName).UploadAsync(args.FileStream, true);
            return args.FileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);

            if (await FileExistsAsync(args, fileName))
            {
                return await GetBlobClient(args, fileName).DeleteIfExistsAsync();
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);

            return await FileExistsAsync(args, fileName);
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);
            if (!await FileExistsAsync(args, fileName))
            {
                return null;
            }

            var blobClient = GetBlobClient(args, fileName);
            var download = await blobClient.DownloadAsync(args.CancellationToken);
            return await TryCopyToMemoryStreamAsync(download.Value.Content, args.CancellationToken);
        }


        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var fileName = AzureFileNameCalculator.Calculate(args);
            if (!await FileExistsAsync(args, fileName))
            {
                return false;
            }
            var blobClient = GetBlobClient(args, fileName);
            var download = await blobClient.DownloadToAsync(args.Path, args.CancellationToken);
            if (!download.IsError)
            {
                await TryWriteToFileAsync(download.Content.ToStream(), args.Path, args.CancellationToken);
            }
            return false;
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
            var configuration = args.Configuration.GetAzureConfiguration();
            var blobServiceClient = new BlobServiceClient(configuration.ConnectionString);
            return blobServiceClient.GetBlobContainerClient(GetContainerName(args));
        }

        protected virtual async Task CreateContainerIfNotExists(FileProviderArgs args)
        {
            var blobContainerClient = GetBlobContainerClient(args);
            await blobContainerClient.CreateIfNotExistsAsync();
        }

        private async Task<bool> FileExistsAsync(FileProviderArgs args, string fileName)
        {
            // Make sure Blob Container exists.
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

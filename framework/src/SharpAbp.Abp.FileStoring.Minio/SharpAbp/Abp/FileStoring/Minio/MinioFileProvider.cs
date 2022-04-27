﻿using Microsoft.Extensions.Logging;
using Minio;
using Minio.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class MinioFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IMinioFileNameCalculator MinioFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        public MinioFileProvider(
            ILogger<MinioFileProvider> logger,
            IClock clock,
            IMinioFileNameCalculator minioFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            Clock = clock;
            MinioFileNameCalculator = minioFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => MinioFileProviderConfigurationNames.ProviderName;

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var fileName = MinioFileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetMinioConfiguration();
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (!args.OverrideExisting && await FileExistsAsync(client, containerName, fileName))
            {
                throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateBucketIfNotExists)
            {
                await CreateBucketIfNotExists(client, containerName);
            }

            await client.PutObjectAsync(containerName, fileName, args.FileStream, args.FileStream.Length);
            return args.FileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var fileName = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (await FileExistsAsync(client, containerName, fileName))
            {
                await client.RemoveObjectAsync(containerName, fileName);
                return true;
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var fileName = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            return await FileExistsAsync(client, containerName, fileName);
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var fileName = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (!await FileExistsAsync(client, containerName, fileName))
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            await client.GetObjectAsync(containerName, fileName, stream =>
            {
                if (stream != null)
                {
                    stream.CopyTo(memoryStream);
                }
                else
                {
                    memoryStream = null;
                }
            });

            return memoryStream;
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var fileName = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (!await FileExistsAsync(client, containerName, fileName))
            {
                return false;
            }

            await client.GetObjectAsync(containerName, fileName, args.Path);

            return true;
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var fileName = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (!await FileExistsAsync(client, containerName, fileName))
            {
                return string.Empty;
            }

            var expiresInt = 600;
            if (args.Expires.HasValue && args.Expires > Clock.Now)
            {
                expiresInt = Convert.ToInt32((args.Expires.Value - Clock.Now).TotalSeconds);
            }
            return await client.PresignedGetObjectAsync(containerName, fileName, expiresInt);
        }


        protected virtual MinioClient GetMinioClient(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetMinioConfiguration();
            var client = new MinioClient(configuration.EndPoint, configuration.AccessKey, configuration.SecretKey);
            if (configuration.WithSSL)
            {
                client.WithSSL();
            }

            return client;
        }

        protected virtual async Task CreateBucketIfNotExists(MinioClient client, string containerName)
        {
            if (!await client.BucketExistsAsync(containerName))
            {
                await client.MakeBucketAsync(containerName);
            }
        }

        private async Task<bool> FileExistsAsync(MinioClient client, string containerName, string fileName)
        {
            // Make sure file Container exists.
            if (await client.BucketExistsAsync(containerName))
            {
                try
                {
                    await client.StatObjectAsync(containerName, fileName);
                }
                catch (Exception e)
                {
                    if (e is ObjectNotFoundException)
                    {
                        return false;
                    }

                    throw;
                }

                return true;
            }

            return false;
        }

        private string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetMinioConfiguration();

            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.BucketName);
        }

    }
}

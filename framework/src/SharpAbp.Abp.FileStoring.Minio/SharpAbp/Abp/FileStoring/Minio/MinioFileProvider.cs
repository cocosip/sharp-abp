using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Minio
{
    [ExposeKeyedService<IFileProvider>(MinioFileProviderConfigurationNames.ProviderName)]
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
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetMinioConfiguration();
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (!args.OverrideExisting && await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateBucketIfNotExists)
            {
                await CreateBucketIfNotExists(client, containerName, args.CancellationToken);
            }

            Check.NotNull(args.FileStream, "args stream");

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(containerName)
                .WithObject(objectKey)
                .WithStreamData(args.FileStream)
                .WithObjectSize(args.FileStream.Length);

            await client.PutObjectAsync(putObjectArgs, args.CancellationToken);

            //await client.PutObjectAsync(containerName, fileName, args.FileStream, args.FileStream.Length);
            return args.FileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(containerName)
                    .WithObject(objectKey);

                await client.RemoveObjectAsync(removeObjectArgs, args.CancellationToken);
                return true;
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            return await FileExistsAsync(client, containerName, objectKey, args.CancellationToken);
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (!await FileExistsAsync(client, containerName, objectKey))
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(containerName)
                .WithObject(objectKey)
                .WithCallbackStream(stream =>
                {
                    if (stream != null)
                    {
                        stream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        memoryStream = null;
                    }
                });

            await client.GetObjectAsync(getObjectArgs, args.CancellationToken);
            return memoryStream;
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (!await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                return false;
            }

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(containerName)
                .WithObject(objectKey)
                .WithFile(args.Path);

            await client.GetObjectAsync(getObjectArgs, args.CancellationToken);
            return true;
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var objectKey = MinioFileNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName(args);

            if (args.CheckFileExist && !await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                return string.Empty;
            }

            var expiresInt = 600;
            if (args.Expires.HasValue && args.Expires > Clock.Now)
            {
                expiresInt = Convert.ToInt32((args.Expires.Value - Clock.Now).TotalSeconds);
            }
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(containerName)
                .WithObject(objectKey)
                .WithExpiry(expiresInt);

            return await client.PresignedGetObjectAsync(presignedGetObjectArgs);
        }


        protected virtual IMinioClient GetMinioClient(FileProviderArgs args)
        {
            var minioConfiguration = args.Configuration.GetMinioConfiguration();
            var client = new MinioClient()
                .WithEndpoint(minioConfiguration.EndPoint)
                .WithCredentials(minioConfiguration.AccessKey, minioConfiguration.SecretKey);
            if (minioConfiguration.WithSSL)
            {
                client.WithSSL();
            }
            client.Build();
            return client;
        }

        protected virtual async Task CreateBucketIfNotExists(
            IMinioClient client,
            string containerName,
            CancellationToken cancellationToken = default)
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(containerName);

            if (!await client.BucketExistsAsync(bucketExistsArgs, cancellationToken))
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(containerName);
                await client.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
        }

        private async Task<bool> FileExistsAsync(
            IMinioClient client,
            string containerName,
            string fileName,
            CancellationToken cancellationToken = default)
        {
            // Make sure file Container exists.

            var bucketExistsArgs = new BucketExistsArgs().WithBucket(containerName);

            if (await client.BucketExistsAsync(bucketExistsArgs, cancellationToken))
            {
                try
                {
                    var statObjectArgs = new StatObjectArgs()
                        .WithBucket(containerName)
                        .WithObject(fileName);
                    await client.StatObjectAsync(statObjectArgs, cancellationToken);
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

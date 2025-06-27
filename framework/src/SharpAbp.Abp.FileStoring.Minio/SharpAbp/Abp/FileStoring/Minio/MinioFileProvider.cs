using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Minio
{
    [ExposeKeyedService<IFileProvider>(MinioFileProviderConfigurationNames.ProviderName)]
    public class MinioFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IClock Clock { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IMinioFileNameCalculator MinioFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        public MinioFileProvider(
            ILogger<MinioFileProvider> logger,
            IServiceProvider serviceProvider,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IClock clock,
            IPoolOrchestrator poolOrchestrator,
            IMinioFileNameCalculator minioFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Options = options.Value;
            Clock = clock;
            PoolOrchestrator = poolOrchestrator;
            MinioFileNameCalculator = minioFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => MinioFileProviderConfigurationNames.ProviderName;

        protected virtual ObjectPool<IMinioClient> GetMinioClientPool(MinioFileProviderConfiguration minioConfiguration)
        {
            var poolName = NormalizePoolName(minioConfiguration);
            var minioClientPolicy = ActivatorUtilities.CreateInstance<MinioClientPolicy>(ServiceProvider, minioConfiguration);
            var pool = PoolOrchestrator.GetPool(poolName, minioClientPolicy, Options.DefaultClientMaximumRetained);
            return pool;
        }

        protected virtual string NormalizePoolName(MinioFileProviderConfiguration minioConfiguration)
        {
            var v = $"{minioConfiguration.EndPoint}-{minioConfiguration.AccessKey}-{minioConfiguration.SecretKey}";
            using var sha1 = SHA1.Create();
            var hashBuffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(v));
            var hash = hashBuffer.Aggregate("", (current, b) => current + b.ToString("X2"));
            return $"FileStoring-{MinioFileProviderConfigurationNames.ProviderName}-{hash}";
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var minioConfiguration = args.Configuration.GetMinioConfiguration();
            var pool = GetMinioClientPool(minioConfiguration);
            var minioClient = pool.Get();

            try
            {

                if (!args.OverrideExisting && await FileExistsAsync(minioClient, containerName, objectKey, args.CancellationToken))
                {
                    throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }

                if (minioConfiguration.CreateBucketIfNotExists)
                {
                    await CreateBucketIfNotExists(minioClient, containerName, args.CancellationToken);
                }

                Check.NotNull(args.FileStream, "args stream");

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(containerName)
                    .WithObject(objectKey)
                    .WithStreamData(args.FileStream)
                    .WithObjectSize(args.FileStream.Length);

                await minioClient.PutObjectAsync(putObjectArgs, args.CancellationToken);

                //await client.PutObjectAsync(containerName, fileName, args.FileStream, args.FileStream.Length);
                return args.FileId;
            }
            finally
            {
                pool.Return(minioClient);
            }
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var minioConfiguration = args.Configuration.GetMinioConfiguration();
            var pool = GetMinioClientPool(minioConfiguration);
            var minioClient = pool.Get();

            try
            {

                if (await FileExistsAsync(minioClient, containerName, objectKey, args.CancellationToken))
                {
                    var removeObjectArgs = new RemoveObjectArgs()
                        .WithBucket(containerName)
                        .WithObject(objectKey);

                    await minioClient.RemoveObjectAsync(removeObjectArgs, args.CancellationToken);
                    return true;
                }

                return false;
            }
            finally
            {
                pool.Return(minioClient);
            }
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var minioConfiguration = args.Configuration.GetMinioConfiguration();
            var pool = GetMinioClientPool(minioConfiguration);
            var minioClient = pool.Get();

            try
            {
                return await FileExistsAsync(minioClient, containerName, objectKey, args.CancellationToken);
            }
            finally
            {
                pool.Return(minioClient);
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var minioConfiguration = args.Configuration.GetMinioConfiguration();
            var pool = GetMinioClientPool(minioConfiguration);
            var minioClient = pool.Get();

            try
            {

                if (!await FileExistsAsync(minioClient, containerName, objectKey))
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

                await minioClient.GetObjectAsync(getObjectArgs, args.CancellationToken);
                return memoryStream;
            }
            finally
            {
                pool.Return(minioClient);
            }
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var objectKey = MinioFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var minioConfiguration = args.Configuration.GetMinioConfiguration();
            var pool = GetMinioClientPool(minioConfiguration);
            var minioClient = pool.Get();

            try
            {

                if (!await FileExistsAsync(minioClient, containerName, objectKey, args.CancellationToken))
                {
                    return false;
                }

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(containerName)
                    .WithObject(objectKey)
                    .WithFile(args.Path);

                await minioClient.GetObjectAsync(getObjectArgs, args.CancellationToken);
                return true;
            }
            finally
            {
                pool.Return(minioClient);
            }
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var objectKey = MinioFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var minioConfiguration = args.Configuration.GetMinioConfiguration();
            var pool = GetMinioClientPool(minioConfiguration);
            var minioClient = pool.Get();

            try
            {

                if (args.CheckFileExist && !await FileExistsAsync(minioClient, containerName, objectKey, args.CancellationToken))
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

                return await minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
            }
            finally
            {
                pool.Return(minioClient);
            }
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
            IMinioClient minioClient,
            string containerName,
            string fileName,
            CancellationToken cancellationToken = default)
        {
            // Make sure file Container exists.

            var bucketExistsArgs = new BucketExistsArgs().WithBucket(containerName);

            if (await minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken))
            {
                try
                {
                    var statObjectArgs = new StatObjectArgs()
                        .WithBucket(containerName)
                        .WithObject(fileName);
                    await minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
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

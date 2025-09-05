using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using OBS;
using OBS.Model;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Obs
{
    [ExposeKeyedService<IFileProvider>(ObsFileProviderConfigurationNames.ProviderName)]
    public class ObsFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IClock Clock { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IObsFileNameCalculator ObsFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public ObsFileProvider(
            ILogger<ObsFileProvider> logger,
            IServiceProvider serviceProvider,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IClock clock,
            IPoolOrchestrator poolOrchestrator,
            IObsFileNameCalculator obsFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Options = options.Value;

            Clock = clock;
            PoolOrchestrator = poolOrchestrator;
            ObsFileNameCalculator = obsFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => ObsFileProviderConfigurationNames.ProviderName;


        protected virtual ObjectPool<ObsClient> GetObsClientPool(ObsFileProviderConfiguration obsConfiguration)
        {
            var poolName = NormalizePoolName(obsConfiguration);
            var obsClientPolicy = ActivatorUtilities.CreateInstance<ObsClientPolicy>(ServiceProvider, obsConfiguration);
            var pool = PoolOrchestrator.GetPool(poolName, obsClientPolicy, Options.DefaultClientMaximumRetained);
            return pool;
        }

        protected virtual string NormalizePoolName(ObsFileProviderConfiguration obsConfiguration)
        {
            var v = $"{obsConfiguration.Endpoint}-{obsConfiguration.AccessKeyId}-{obsConfiguration.AccessKeySecret}";
            using var sha1 = SHA1.Create();
            var hashBuffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(v));
            var hash = hashBuffer.Aggregate("", (current, b) => current + b.ToString("X2"));
            return $"FileStoring-{ObsFileProviderConfigurationNames.ProviderName}-{hash}";
        }


        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var pool = GetObsClientPool(obsConfiguration);
            var obsClient = pool.Get();

            try
            {
                if (!args.OverrideExisting && FileExistsAsync(obsClient, containerName, objectKey))
                {
                    throw new FileAlreadyExistsException($"Saving FILE '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }
                if (obsConfiguration.CreateContainerIfNotExists)
                {
                    if (!obsClient.HeadBucket(new HeadBucketRequest() { BucketName = containerName }))
                    {
                        obsClient.CreateBucket(new CreateBucketRequest() { BucketName = containerName });
                    }
                }

                if (args.Configuration.EnableAutoMultiPartUpload && args.FileStream!.Length > args.Configuration.MultiPartUploadMinFileSize)
                {
                    return await MultiPartUploadAsync(obsClient, containerName, objectKey, args);
                }
                else
                {
                    return await PutObjectAsync(obsClient, containerName, objectKey, args);
                }
            }
            finally
            {
                pool.Return(obsClient);
            }
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var pool = GetObsClientPool(obsConfiguration);
            var obsClient = pool.Get();
            try
            {

                if (!FileExistsAsync(obsClient, containerName, objectKey))
                {
                    return Task.FromResult(false);
                }
                obsClient.DeleteObject(new DeleteObjectRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey
                });
                return Task.FromResult(true);
            }
            finally
            {
                pool.Return(obsClient);
            }
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var pool = GetObsClientPool(obsConfiguration);
            var obsClient = pool.Get();

            try
            {

                return Task.FromResult(FileExistsAsync(obsClient, containerName, objectKey));
            }
            finally
            {
                pool.Return(obsClient);
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var pool = GetObsClientPool(obsConfiguration);
            var obsClient = pool.Get();

            try
            {
                if (!FileExistsAsync(obsClient, containerName, objectKey))
                {
                    return null;
                }
                var result = obsClient.GetObject(new GetObjectRequest() { BucketName = containerName, ObjectKey = objectKey });

                return await TryCopyToMemoryStreamAsync(result.OutputStream, args.CancellationToken);
            }
            finally
            {
                pool.Return(obsClient);
            }
        }


        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var pool = GetObsClientPool(obsConfiguration);
            var obsClient = pool.Get();

            try
            {
                if (!FileExistsAsync(obsClient, containerName, objectKey))
                {
                    return false;
                }

                var result = obsClient.GetObject(new GetObjectRequest() { BucketName = containerName, ObjectKey = objectKey });
                await TryWriteToFileAsync(result.OutputStream, args.Path, args.CancellationToken);
                return true;
            }
            finally
            {
                pool.Return(obsClient);
            }
        }


        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return Task.FromResult(string.Empty);
            }

            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var pool = GetObsClientPool(obsConfiguration);
            var obsClient = pool.Get();

            try
            {
                if (args.CheckFileExist && !FileExistsAsync(obsClient, containerName, objectKey))
                {
                    return Task.FromResult(string.Empty);
                }

                var expiresSeconds = 600;
                if (args.Expires.HasValue)
                {
                    expiresSeconds = (int)(args.Expires.Value - Clock.Now).TotalSeconds;
                }

                var temporarySignatureResponse = obsClient.CreateTemporarySignature(new CreateTemporarySignatureRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey,
                    Expires = expiresSeconds
                });

                return Task.FromResult(temporarySignatureResponse.SignUrl);
            }
            finally
            {
                pool.Return(obsClient);
            }
        }

        protected virtual string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetObsConfiguration();
            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.BucketName);
        }

        protected virtual bool FileExistsAsync(ObsClient obsClient, string containerName, string fileName)
        {
            // Make sure Blob Container exists.
            var headBucketRequest = new HeadBucketRequest()
            {
                BucketName = containerName
            };

            var hadObjectRequest = new HeadObjectRequest()
            {
                BucketName = containerName,
                ObjectKey = fileName
            };

            return obsClient.HeadBucket(headBucketRequest) &&
                   obsClient.HeadObject(hadObjectRequest);
        }

        protected virtual Task<string> PutObjectAsync(
            ObsClient obsClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            obsClient.PutObject(new PutObjectRequest
            {
                BucketName = containerName,
                ObjectKey = objectKey,
                InputStream = args.FileStream,
            });
            return Task.FromResult(args.FileId);
        }

        protected virtual async Task<string> MultiPartUploadAsync(
            ObsClient obsClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var initiateMultipartUploadResponse = obsClient.InitiateMultipartUpload(new InitiateMultipartUploadRequest()
            {
                BucketName = containerName,
                ObjectKey = objectKey
            });

            // Upload ID
            var uploadId = initiateMultipartUploadResponse.UploadId;
            // Calculate the total number of shards
            var partSize = args.Configuration.MultiPartUploadShardingSize;

            var fileSize = args.FileStream!.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            // Start multipart upload. partETags is a list that stores partETag. After OBS receives the shard list submitted by the user,
            // it will verify the validity of each shard data one by one. After all data shards pass the verification,
            // OBS will combine these shards into a complete file.
            var partETags = new List<PartETag>();

            for (var i = 0; i < partCount; i++)
            {
                var skipBytes = (long)partSize * i;
                args.FileStream.Seek(skipBytes, 0);

                // Calculate the size of the shard to be uploaded this time. The last shard is the remaining data size.
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var buffer = new byte[size];
                await args.FileStream.ReadAsync(buffer, 0, buffer.Length);

                var request = new UploadPartRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey,
                    UploadId = uploadId,
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1
                };
                // Call the UploadPart interface to perform the upload function. The result contains the ETag value of this data shard.
                var result = obsClient.UploadPart(request);
                partETags.Add(new PartETag(result.PartNumber, result.ETag));

                Logger.LogDebug("Multipart upload progress for file '{FileId}' with UploadId '{UploadId}': {CompletedParts}/{TotalParts} completed.", 
                    args.FileId, uploadId, partETags.Count, partCount);
            }

            var completeMultipartUploadResponse = obsClient.CompleteMultipartUpload(new CompleteMultipartUploadRequest()
            {
                BucketName = containerName,
                ObjectKey = objectKey,
                UploadId = uploadId,
                PartETags = partETags
            });

            Logger.LogInformation("Multipart upload completed for file '{FileId}' with key '{ObjectKey}'. ETag: {ETag}", 
                args.FileId, completeMultipartUploadResponse.ObjectKey, completeMultipartUploadResponse.ETag);
            return args.FileId;
        }


    }
}

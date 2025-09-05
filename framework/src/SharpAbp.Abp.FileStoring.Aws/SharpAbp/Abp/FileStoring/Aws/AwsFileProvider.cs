using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Aws
{
    [ExposeKeyedService<IFileProvider>(AwsFileProviderConfigurationNames.ProviderName)]
    public class AwsFileProvider : FileProviderBase, ITransientDependency
    {
        public override string Provider => AwsFileProviderConfigurationNames.ProviderName;

        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IClock Clock { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IAwsFileNameCalculator AwsFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        public AwsFileProvider(
            ILogger<AwsFileProvider> logger,
            IServiceProvider serviceProvider,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IClock clock,
            IPoolOrchestrator poolOrchestrator,
            IAwsFileNameCalculator awsFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Options = options.Value;
            Clock = clock;
            PoolOrchestrator = poolOrchestrator;
            AwsFileNameCalculator = awsFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        protected virtual ObjectPool<IAmazonS3> GetAmazonS3Pool(AwsFileProviderConfiguration awsConfiguration)
        {
            var poolName = NormalizePoolName(awsConfiguration);
            var amazonS3ClientPolicy = ActivatorUtilities.CreateInstance<AmazonS3ClientPolicy>(ServiceProvider, awsConfiguration);
            var pool = PoolOrchestrator.GetPool(poolName, amazonS3ClientPolicy, Options.DefaultClientMaximumRetained);
            return pool;
        }


        protected virtual string NormalizePoolName(AwsFileProviderConfiguration awsConfiguration)
        {
            var v = $"{awsConfiguration.Region}-{awsConfiguration.AccessKeyId}-{awsConfiguration.SecretAccessKey}";
            using var sha1 = SHA1.Create();
            var hashBuffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(v));
            var hash = hashBuffer.Aggregate("", (current, b) => current + b.ToString("X2"));
            return $"FileStoring-{AwsFileProviderConfigurationNames.ProviderName}-{hash}";
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var objectKey = AwsFileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetAwsConfiguration();
            var containerName = GetContainerName(args);

            var awsConfiguration = args.Configuration.GetAwsConfiguration();
            var pool = GetAmazonS3Pool(awsConfiguration);
            var amazonS3Client = pool.Get();

            try
            {

                if (!args.OverrideExisting && await FileExistsAsync(amazonS3Client, containerName, objectKey))
                {
                    throw new FileAlreadyExistsException(
                        $"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }

                if (configuration.CreateContainerIfNotExists)
                {
                    await CreateContainerIfNotExists(amazonS3Client, containerName);
                }

                if (args.Configuration.EnableAutoMultiPartUpload && args.FileStream!.Length > args.Configuration.MultiPartUploadMinFileSize)
                {
                    return await MultiPartUploadAsync(amazonS3Client, containerName, objectKey, args);
                }
                else
                {
                    return await PutObjectAsync(amazonS3Client, containerName, objectKey, args);
                }

            }
            finally
            {
                pool.Return(amazonS3Client);
            }
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var objectKey = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var awsConfiguration = args.Configuration.GetAwsConfiguration();
            var pool = GetAmazonS3Pool(awsConfiguration);
            var amazonS3Client = pool.Get();

            try
            {

                if (!await FileExistsAsync(amazonS3Client, containerName, objectKey))
                {
                    return false;
                }

                await amazonS3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = containerName,
                    Key = objectKey
                }, args.CancellationToken);

                return true;
            }
            finally
            {
                pool.Return(amazonS3Client);
            }
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var objectKey = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var awsConfiguration = args.Configuration.GetAwsConfiguration();
            var pool = GetAmazonS3Pool(awsConfiguration);
            var amazonS3Client = pool.Get();

            try
            {

                return await FileExistsAsync(amazonS3Client, containerName, objectKey);
            }
            finally
            {
                pool.Return(amazonS3Client);
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var objectKey = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var awsConfiguration = args.Configuration.GetAwsConfiguration();
            var pool = GetAmazonS3Pool(awsConfiguration);
            var amazonS3Client = pool.Get();

            try
            {
                if (!await FileExistsAsync(amazonS3Client, containerName, objectKey))
                {
                    return null;
                }

                var response = await amazonS3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = containerName,
                    Key = objectKey
                }, args.CancellationToken);

                return await TryCopyToMemoryStreamAsync(response.ResponseStream, args.CancellationToken);
            }
            finally
            {
                pool.Return(amazonS3Client);
            }
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var objectKey = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var awsConfiguration = args.Configuration.GetAwsConfiguration();
            var pool = GetAmazonS3Pool(awsConfiguration);
            var amazonS3Client = pool.Get();

            try
            {
                if (!await FileExistsAsync(amazonS3Client, containerName, objectKey))
                {
                    return false;
                }

                var response = await amazonS3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = containerName,
                    Key = objectKey
                }, args.CancellationToken);

                await TryWriteToFileAsync(response.ResponseStream, args.Path, args.CancellationToken);
                return true;
            }
            finally
            {
                pool.Return(amazonS3Client);
            }
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var objectKey = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            var awsConfiguration = args.Configuration.GetAwsConfiguration();
            var pool = GetAmazonS3Pool(awsConfiguration);
            var amazonS3Client = pool.Get();

            try
            {
                if (args.CheckFileExist && !await FileExistsAsync(amazonS3Client, containerName, objectKey))
                {
                    return string.Empty;
                }

                var datetime = args.Expires ?? Clock.Now.AddSeconds(600);
                var url = amazonS3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
                {
                    BucketName = containerName,
                    Key = objectKey,
                    Expires = datetime
                });

                return url;
            }
            finally
            {
                pool.Return(amazonS3Client);
            }
        }


        protected virtual async Task<bool> FileExistsAsync(
            IAmazonS3 amazonS3Client,
            string containerName,
            string fileName)
        {
            // Make sure file Container exists.
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, containerName))
            {
                return false;
            }

            try
            {
                await amazonS3Client.GetObjectMetadataAsync(containerName, fileName);
            }
            catch (Exception ex)
            {
                if (ex is AmazonS3Exception)
                {
                    return false;
                }

                throw;
            }
            return true;
        }

        protected virtual async Task CreateContainerIfNotExists(
            IAmazonS3 amazonS3Client,
            string containerName)
        {
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, containerName))
            {
                await amazonS3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = containerName
                });
            }
        }

 

        protected virtual string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAwsConfiguration();
            return configuration.ContainerName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.ContainerName);
        }

        protected virtual async Task<string> PutObjectAsync(
            IAmazonS3 amazonS3Client,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            await amazonS3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = containerName,
                Key = objectKey,
                InputStream = args.FileStream
            }, args.CancellationToken);
            return args.FileId;
        }

        protected virtual async Task<string> MultiPartUploadAsync(
            IAmazonS3 amazonS3Client,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var configuration = args.Configuration.GetAwsConfiguration();
            var initiateMultipartUploadResponse = await amazonS3Client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest()
            {
                BucketName = containerName,
                Key = objectKey
            }, args.CancellationToken);

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

            // Start multipart upload. partETags is a list that stores partETag. After S3 receives the shard list submitted by the user, 
            // it will verify the validity of each shard data one by one. After all data shards pass the verification, 
            // S3 will combine these shards into a complete file.
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
                    Key = objectKey,
                    UploadId = uploadId,
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1,
                };

                // Call the UploadPart interface to perform the upload function. The result contains the ETag value of this data shard.
                var result = await amazonS3Client.UploadPartAsync(request, args.CancellationToken);
                partETags.Add(new PartETag(result.PartNumber, result.ETag));

                Logger.LogDebug("Multipart upload progress for file '{FileId}' with UploadId '{UploadId}': {CompletedParts}/{TotalParts} completed.", 
                    args.FileId, uploadId, partETags.Count, partCount);
            }

            var completeMultipartUploadResponse = await amazonS3Client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest()
            {
                BucketName = containerName,
                Key = objectKey,
                UploadId = uploadId,
                PartETags = partETags
            }, args.CancellationToken);

            Logger.LogInformation("Multipart upload completed for file '{FileId}' with key '{ObjectKey}'. ETag: {ETag}", 
                args.FileId, completeMultipartUploadResponse.Key, completeMultipartUploadResponse.ETag);

            return args.FileId;
        }

 

    }
}

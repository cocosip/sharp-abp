using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.S3
{
    [ExposeKeyedService<IFileProvider>(S3FileProviderConfigurationNames.ProviderName)]
    public class S3FileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IClock Clock { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IS3FileNameCalculator FileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public S3FileProvider(
            ILogger<S3FileProvider> logger,
            IServiceProvider serviceProvider,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IClock clock,
            IPoolOrchestrator poolOrchestrator,
            IS3FileNameCalculator fileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Options = options.Value;
            Clock = clock;
            PoolOrchestrator = poolOrchestrator;
            FileNameCalculator = fileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => S3FileProviderConfigurationNames.ProviderName;

        protected virtual ObjectPool<IAmazonS3> GetS3ClientPool(S3FileProviderConfiguration s3Configuration)
        {
            var poolName = NormalizePoolName(s3Configuration);
            var aliyunOssClientPolicy = ActivatorUtilities.CreateInstance<S3ClientPolicy>(ServiceProvider, s3Configuration);
            var pool = PoolOrchestrator.GetPool(poolName, aliyunOssClientPolicy, Options.DefaultClientMaximumRetained);
            return pool;
        }

        protected virtual string NormalizePoolName(S3FileProviderConfiguration s3Configuration)
        {
            var v = $"{s3Configuration.ServerUrl.TrimEnd('/')}-{s3Configuration.AccessKeyId}-{s3Configuration.SecretAccessKey}";
            using var sha1 = SHA1.Create();
            var hashBuffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(v));
            var hash = hashBuffer.Aggregate("", (current, b) => current + b.ToString("X2"));
            return $"FileStoring-{S3FileProviderConfigurationNames.ProviderName}-{hash}";
        }



        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);

            var s3Configuration = args.Configuration.GetS3Configuration();
            var pool = GetS3ClientPool(s3Configuration);
            var s3Client = pool.Get();

            try
            {

                if (!args.OverrideExisting && await FileExistsAsync(s3Client, containerName, objectKey, args.CancellationToken))
                {
                    throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }

                if (s3Configuration.CreateBucketIfNotExists)
                {
                    await CreateBucketIfNotExists(s3Client, containerName);
                }

                string fileId;
                if (args.Configuration.EnableAutoMultiPartUpload && (args.FileStream!.Length > args.Configuration.MultiPartUploadMinFileSize))
                {
                    fileId = await MultipartUploadAsync(s3Client, containerName, objectKey, s3Configuration, args);
                }
                else
                {
                    fileId = await SingleUploadAsync(s3Client, containerName, objectKey, s3Configuration, args);
                }

                args.FileStream?.Dispose();

                return fileId;
            }
            finally
            {
                pool.Return(s3Client);
            }
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);

            var s3Configuration = args.Configuration.GetS3Configuration();
            var pool = GetS3ClientPool(s3Configuration);
            var s3Client = pool.Get();

            try
            {

                if (await FileExistsAsync(s3Client, containerName, objectKey, args.CancellationToken))
                {
                    await s3Client.DeleteAsync(containerName, objectKey, null, args.CancellationToken);
                    return true;
                }

                return false;

            }
            finally
            {
                pool.Return(s3Client);
            }
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);

            var s3Configuration = args.Configuration.GetS3Configuration();
            var pool = GetS3ClientPool(s3Configuration);
            var s3Client = pool.Get();

            try
            {
                return await FileExistsAsync(s3Client, containerName, objectKey, args.CancellationToken);

            }
            finally
            {
                pool.Return(s3Client);
            }
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);

            var s3Configuration = args.Configuration.GetS3Configuration();
            var pool = GetS3ClientPool(s3Configuration);
            var s3Client = pool.Get();

            try
            {
                var getObjectResponse = await s3Client.GetObjectAsync(containerName, objectKey, args.CancellationToken);
                await getObjectResponse.WriteResponseStreamToFileAsync(args.Path, true, args.CancellationToken);
                return true;

            }
            finally
            {
                pool.Return(s3Client);
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);

            var s3Configuration = args.Configuration.GetS3Configuration();
            var pool = GetS3ClientPool(s3Configuration);
            var s3Client = pool.Get();

            try
            {

                if (!await FileExistsAsync(s3Client, containerName, objectKey, args.CancellationToken))
                {
                    return null;
                }

                var getObjectResponse = await s3Client.GetObjectAsync(containerName, objectKey, args.CancellationToken);
                return await TryCopyToMemoryStreamAsync(getObjectResponse.ResponseStream, args.CancellationToken);
            }
            finally
            {
                pool.Return(s3Client);
            }
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);

            var s3Configuration = args.Configuration.GetS3Configuration();
            var pool = GetS3ClientPool(s3Configuration);
            var s3Client = pool.Get();

            try
            {

                if (args.CheckFileExist && !await FileExistsAsync(s3Client, containerName, objectKey, args.CancellationToken))
                {
                    return string.Empty;
                }

                var preSignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = containerName,
                    Key = objectKey,
                    Protocol = (Protocol)s3Configuration.Protocol,
                    Expires = args.Expires ?? Clock.Now.AddSeconds(600)
                };

                var accessUrl = s3Client.GetPreSignedURL(preSignedUrlRequest);

                return accessUrl;
            }
            finally
            {
                pool.Return(s3Client);
            }
        }


        /// <summary>
        /// Upload a single file
        /// </summary>
        protected virtual async Task<string> SingleUploadAsync(
            IAmazonS3 client,
            string containerName,
            string objectKey,
            S3FileProviderConfiguration configuration,
            FileProviderSaveArgs args)
        {
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = containerName,
                Key = objectKey,
                InputStream = args.FileStream,
                AutoCloseStream = true,
                UseChunkEncoding = configuration.UseChunkEncoding,
            };
            await client.PutObjectAsync(putObjectRequest, args.CancellationToken);
            Logger.LogInformation("Successfully uploaded single file '{FileId}' to S3. Bucket: {BucketName}, Key: {ObjectKey}", 
                args.FileId, containerName, objectKey);
            return args.FileId;
        }

        protected virtual async Task<string> MultipartUploadAsync(
            IAmazonS3 client,
            string containerName,
            string objectKey,
            S3FileProviderConfiguration configuration,
            FileProviderSaveArgs args)
        {
            var initiateMultipartUploadResponse = await client.InitiateMultipartUploadAsync(containerName, objectKey, args.CancellationToken);
            // Upload ID
            var uploadId = initiateMultipartUploadResponse.UploadId;
            // Calculate slice part count
            var partSize = args.Configuration.MultiPartUploadShardingSize;
            //var fi = new FileInfo(spoolFile.FilePath);//?
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
                // Calculate the size of the shard to be uploaded this time. The last shard is the remaining data size.
                var size = (int)((partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes));

                byte[] buffer = new byte[size];
                args.FileStream.Read(buffer, 0, size);

                // Upload part
                var uploadPartResponse = await client.UploadPartAsync(new UploadPartRequest()
                {
                    BucketName = containerName,
                    UploadId = uploadId,
                    Key = objectKey,
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1,
                    UseChunkEncoding = configuration.UseChunkEncoding,
                }, args.CancellationToken);
                partETags.Add(new PartETag(uploadPartResponse.PartNumber, uploadPartResponse.ETag));
                Logger.LogDebug("Multipart upload progress for file '{FileId}' with UploadId '{UploadId}': {CompletedParts}/{TotalParts} completed. Bucket: {BucketName}, Key: {ObjectKey}", 
                    args.FileId, uploadId, partETags.Count, partCount, containerName, objectKey);
            }

            // Complete multipart upload
            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest()
            {
                BucketName = containerName,
                Key = objectKey,
                UploadId = uploadId,
                PartETags = partETags
            };

            var completeMultipartUploadResponse = await client.CompleteMultipartUploadAsync(completeMultipartUploadRequest, args.CancellationToken);
            Logger.LogInformation("Multipart upload completed for file '{FileId}' with key '{ObjectKey}'. ETag: {ETag}, Bucket: {BucketName}", 
                args.FileId, completeMultipartUploadResponse.Key, completeMultipartUploadResponse.ETag, containerName);
            return args.FileId;
        }




        protected virtual async Task CreateBucketIfNotExists(IAmazonS3 client, string containerName)
        {
            await client.EnsureBucketExistsAsync(containerName);
        }

        private async Task<bool> FileExistsAsync(
            IAmazonS3 client,
            string containerName,
            string fileId,
            CancellationToken cancellationToken)
        {
            // Make sure Blob Container exists.
            if (await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(client, containerName))
            {
                try
                {
                    var getObjectMetadataResponse = await client.GetObjectMetadataAsync(containerName, fileId, cancellationToken);
                    return getObjectMetadataResponse.HttpStatusCode == HttpStatusCode.OK;

                }
                catch (Exception e)
                {
                    if (e is AmazonS3Exception)
                    {
                        return false;
                    }

                    throw;
                }
            }

            return false;
        }

        private string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetS3Configuration();

            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.BucketName);
        }
    }
}

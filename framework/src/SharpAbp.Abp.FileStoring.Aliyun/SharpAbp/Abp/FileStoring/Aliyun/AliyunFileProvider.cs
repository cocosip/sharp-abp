using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    [ExposeKeyedService<IFileProvider>(AliyunFileProviderConfigurationNames.ProviderName)]
    public class AliyunFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IClock Clock { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IAliyunFileNameCalculator AliyunFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public AliyunFileProvider(
            ILogger<AliyunFileProvider> logger,
            IServiceProvider serviceProvider,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IClock clock,
            IPoolOrchestrator poolOrchestrator,
            IAliyunFileNameCalculator aliyunFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Options = options.Value;
            Clock = clock;
            PoolOrchestrator = poolOrchestrator;
            AliyunFileNameCalculator = aliyunFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => AliyunFileProviderConfigurationNames.ProviderName;


        protected virtual ObjectPool<IOss> GetOssClientPool(AliyunFileProviderConfiguration aliyunConfiguration)
        {
            var poolName = NormalizePoolName(aliyunConfiguration);
            var aliyunOssClientPolicy = ActivatorUtilities.CreateInstance<AliyunOssClientPolicy>(ServiceProvider, aliyunConfiguration);
            var pool = PoolOrchestrator.GetPool(poolName, aliyunOssClientPolicy, Options.DefaultClientMaximumRetained);
            return pool;
        }

        protected virtual string NormalizePoolName(AliyunFileProviderConfiguration aliyunConfiguration)
        {
            var v = $"{aliyunConfiguration.Endpoint}-{aliyunConfiguration.AccessKeyId}-{aliyunConfiguration.AccessKeySecret}";
            using var sha1 = SHA1.Create();
            var hashBuffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(v));
            var hash = hashBuffer.Aggregate("", (current, b) => current + b.ToString("X2"));
            return $"FileStoring-{AliyunFileProviderConfigurationNames.ProviderName}-{hash}";
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var pool = GetOssClientPool(aliyunConfiguration);
            var ossClient = pool.Get();
            try
            {

                if (!args.OverrideExisting && FileExistsAsync(ossClient, containerName, objectKey))
                {
                    throw new FileAlreadyExistsException($"Saving FILE '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }
                if (aliyunConfiguration.CreateContainerIfNotExists)
                {
                    if (!ossClient.DoesBucketExist(containerName))
                    {
                        ossClient.CreateBucket(containerName);
                    }
                }

                if (args.Configuration.EnableAutoMultiPartUpload && args.FileStream?.Length > args.Configuration.MultiPartUploadMinFileSize)
                {
                    return await MultiPartUploadAsync(ossClient, containerName, objectKey, args);
                }
                else
                {
                    return await PutObjectAsync(ossClient, containerName, objectKey, args);
                }
            }
            finally
            {
                pool.Return(ossClient);
            }
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var pool = GetOssClientPool(aliyunConfiguration);
            var ossClient = pool.Get();
            try
            {

                if (!FileExistsAsync(ossClient, containerName, objectKey))
                {
                    return Task.FromResult(false);
                }
                ossClient.DeleteObject(containerName, objectKey);
                return Task.FromResult(true);
            }
            finally
            {
                pool.Return(ossClient);
            }
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var pool = GetOssClientPool(aliyunConfiguration);
            var ossClient = pool.Get();

            try
            {
                return Task.FromResult(FileExistsAsync(ossClient, containerName, objectKey));
            }
            finally
            {
                pool.Return(ossClient);
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var pool = GetOssClientPool(aliyunConfiguration);
            var ossClient = pool.Get();

            try
            {
                if (!FileExistsAsync(ossClient, containerName, objectKey))
                {
                    return null;
                }
                var result = ossClient.GetObject(containerName, objectKey);

                return await TryCopyToMemoryStreamAsync(result.Content, args.CancellationToken);
            }
            finally
            {
                pool.Return(ossClient);
            }
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var pool = GetOssClientPool(aliyunConfiguration);
            var ossClient = pool.Get();

            try
            {
                if (!FileExistsAsync(ossClient, containerName, objectKey))
                {
                    return false;
                }

                var result = ossClient.GetObject(containerName, objectKey);
                await TryWriteToFileAsync(result.Content, args.Path, args.CancellationToken);
                return true;
            }
            finally
            {
                pool.Return(ossClient);
            }
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return Task.FromResult(string.Empty);
            }

            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var pool = GetOssClientPool(aliyunConfiguration);
            var ossClient = pool.Get();

            try
            {

                if (args.CheckFileExist && !FileExistsAsync(ossClient, containerName, objectKey))
                {
                    return Task.FromResult(string.Empty);
                }

                var datetime = args.Expires ?? Clock.Now.AddSeconds(600);
                var uri = ossClient.GeneratePresignedUri(containerName, objectKey, datetime);

                return Task.FromResult(uri.ToString());
            }
            finally
            {
                pool.Return(ossClient);
            }
        }

        protected virtual string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAliyunConfiguration();
            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.BucketName);
        }

        protected virtual bool FileExistsAsync(IOss ossClient, string containerName, string fileName)
        {
            // Make sure Blob Container exists.
            return ossClient.DoesBucketExist(containerName) &&
                   ossClient.DoesObjectExist(containerName, fileName);
        }


        protected virtual Task<string> PutObjectAsync(
            IOss ossClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            ossClient.PutObject(containerName, objectKey, args.FileStream);
            return Task.FromResult(args.FileId);
        }


        protected virtual async Task<string> MultiPartUploadAsync(
            IOss ossClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var initiateMultipartUploadResult = ossClient.InitiateMultipartUpload(new InitiateMultipartUploadRequest(containerName, objectKey));

            // Upload ID
            var uploadId = initiateMultipartUploadResult.UploadId;
            // Calculate the total number of shards
            var partSize = args.Configuration.MultiPartUploadShardingSize;
            //var fi = new FileInfo(spoolFile.FilePath);//?
            var fileSize = args.FileStream!.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            // Start multipart upload. partETags is a list that stores partETag. After OSS receives the shard list submitted by the user, 
            // it will verify the validity of each shard data one by one. After all data shards pass the verification, 
            // OSS will combine these shards into a complete file.
            var partETags = new List<PartETag>();

            for (var i = 0; i < partCount; i++)
            {
                var skipBytes = (long)partSize * i;
                args.FileStream.Seek(skipBytes, 0);

                // Calculate the size of the shard to be uploaded this time. The last shard is the remaining data size.
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var buffer = new byte[size];
                await args.FileStream.ReadAsync(buffer, 0, buffer.Length);

                var request = new UploadPartRequest(containerName, objectKey, uploadId)
                {
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1
                };
                // Call the UploadPart interface to perform the upload function. The result contains the ETag value of this data shard.
                var result = ossClient.UploadPart(request);
                partETags.Add(result.PartETag);

                Logger.LogDebug("Multipart upload progress for file '{FileId}' with UploadId '{UploadId}': {CompletedParts}/{TotalParts} completed.", 
                    args.FileId, uploadId, partETags.Count, partCount);
            }
            
            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(containerName, objectKey, uploadId);
            foreach (var partETag in partETags)
            {
                completeMultipartUploadRequest.PartETags.Add(partETag);
            }

            var completeMultipartUploadResult = ossClient.CompleteMultipartUpload(completeMultipartUploadRequest);
            Logger.LogInformation("Multipart upload completed for file '{FileId}' with key '{ObjectKey}'. ETag: {ETag}", 
                args.FileId, completeMultipartUploadResult.Key, completeMultipartUploadResult.ETag);
            return args.FileId;
        }


    }
}

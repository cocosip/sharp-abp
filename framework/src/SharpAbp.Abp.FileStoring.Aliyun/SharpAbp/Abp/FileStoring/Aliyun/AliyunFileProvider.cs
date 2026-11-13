using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aliyun.OSS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    [ExposeKeyedService<IFileProvider>(AliyunFileProviderConfigurationNames.ProviderName)]
    public class AliyunFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected IClock Clock { get; }
        protected FileProviderClientCache<IOss> ClientCache { get; }
        protected IAliyunFileNameCalculator AliyunFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public AliyunFileProvider(
            ILogger<AliyunFileProvider> logger,
            IServiceScopeFactory serviceScopeFactory,
            IClock clock,
            FileProviderClientCache<IOss> clientCache,
            IAliyunFileNameCalculator aliyunFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceScopeFactory = serviceScopeFactory;
            Clock = clock;
            ClientCache = clientCache;
            AliyunFileNameCalculator = aliyunFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => AliyunFileProviderConfigurationNames.ProviderName;


        protected virtual OssClientLease GetOssClient(AliyunFileProviderConfiguration aliyunConfiguration)
        {
            if (!ShouldCacheClient(aliyunConfiguration))
            {
                var client = new AliyunOssClientPolicy(ServiceScopeFactory, aliyunConfiguration).Create();
                return new OssClientLease(client, true);
            }

            var cachedClient = ClientCache.GetOrAdd(
                BuildClientKey(aliyunConfiguration),
                () => new AliyunOssClientPolicy(ServiceScopeFactory, aliyunConfiguration).Create());

            return new OssClientLease(cachedClient, false);
        }

        protected virtual bool ShouldCacheClient(AliyunFileProviderConfiguration aliyunConfiguration)
        {
            return !aliyunConfiguration.UseSecurityTokenService;
        }

        protected virtual string BuildClientKey(AliyunFileProviderConfiguration aliyunConfiguration)
        {
            return FileProviderClientKey.Build(
                Provider,
                ("Endpoint", aliyunConfiguration.Endpoint),
                ("AccessKeyId", aliyunConfiguration.AccessKeyId),
                ("AccessKeySecret", aliyunConfiguration.AccessKeySecret));
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var lease = GetOssClient(aliyunConfiguration);
            var ossClient = lease.Client;
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
                lease.Return();
            }
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var lease = GetOssClient(aliyunConfiguration);
            var ossClient = lease.Client;
            try
            {

                if (!FileExistsAsync(ossClient, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in Aliyun OSS when deleting. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return Task.FromResult(false);
                }
                ossClient.DeleteObject(containerName, objectKey);
                return Task.FromResult(true);
            }
            finally
            {
                lease.Return();
            }
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var lease = GetOssClient(aliyunConfiguration);
            var ossClient = lease.Client;

            try
            {
                return Task.FromResult(FileExistsAsync(ossClient, containerName, objectKey));
            }
            finally
            {
                lease.Return();
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var lease = GetOssClient(aliyunConfiguration);
            var ossClient = lease.Client;

            try
            {
                if (!FileExistsAsync(ossClient, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in Aliyun OSS. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return null;
                }
                using var result = await GetObjectAsync(ossClient, containerName, objectKey);

                return await TryCopyToMemoryStreamAsync(result.Content, args.CancellationToken);
            }
            finally
            {
                lease.Return();
            }
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);

            var aliyunConfiguration = args.Configuration.GetAliyunConfiguration();
            var lease = GetOssClient(aliyunConfiguration);
            var ossClient = lease.Client;

            try
            {
                if (!FileExistsAsync(ossClient, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in Aliyun OSS. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return false;
                }

                using var result = await GetObjectAsync(ossClient, containerName, objectKey);
                await TryWriteToFileAsync(result.Content, args.Path, args.CancellationToken);
                return true;
            }
            finally
            {
                lease.Return();
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
            var lease = GetOssClient(aliyunConfiguration);
            var ossClient = lease.Client;

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
                lease.Return();
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


        protected virtual async Task<string> PutObjectAsync(
            IOss ossClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var request = new PutObjectRequest(containerName, objectKey, args.FileStream);
            await Task<PutObjectResult>.Factory.FromAsync(
                (callback, state) => ossClient.BeginPutObject(request, callback, state),
                ossClient.EndPutObject,
                null);

            return args.FileId;
        }

        protected virtual Task<OssObject> GetObjectAsync(IOss ossClient, string containerName, string objectKey)
        {
            var request = new GetObjectRequest(containerName, objectKey);
            return Task<OssObject>.Factory.FromAsync(
                (callback, state) => ossClient.BeginGetObject(request, callback, state),
                ossClient.EndGetObject,
                null);
        }


        protected virtual async Task<string> MultiPartUploadAsync(
            IOss ossClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var initiateMultipartUploadResult = ossClient.InitiateMultipartUpload(new InitiateMultipartUploadRequest(containerName, objectKey));
            var uploadId = initiateMultipartUploadResult.UploadId;

            try
            {
                // Calculate the total number of shards
                var partSize = args.Configuration.MultiPartUploadShardingSize;
                var initialPosition = args.FileStream!.CanSeek ? args.FileStream.Position : 0;
                var fileSize = args.FileStream.Length - initialPosition;
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
                    if (args.FileStream.CanSeek)
                    {
                        args.FileStream.Seek(initialPosition + skipBytes, SeekOrigin.Begin);
                    }

                    // Calculate the size of the shard to be uploaded this time. The last shard is the remaining data size.
                    var size = (int)((partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes));
                    var buffer = new byte[size];
                    var bytesRead = await ReadToBufferAsync(args.FileStream, buffer, size, args.CancellationToken);
                    if (bytesRead != size)
                    {
                        throw new EndOfStreamException($"Unable to read multipart segment {i + 1} for file '{args.FileId}'. Expected {size} bytes, read {bytesRead} bytes.");
                    }

                    using var partStream = new MemoryStream(buffer);
                    var request = new UploadPartRequest(containerName, objectKey, uploadId)
                    {
                        InputStream = partStream,
                        PartSize = size,
                        PartNumber = i + 1
                    };
                    // Call the UploadPart interface to perform the upload function. The result contains the ETag value of this data shard.
                    var result = await Task<UploadPartResult>.Factory.FromAsync(
                        (callback, state) => ossClient.BeginUploadPart(request, callback, state),
                        ossClient.EndUploadPart,
                        null);
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
            catch (Exception ex)
            {
                Logger.LogError(ex, "Multipart upload failed for file '{FileId}' with UploadId '{UploadId}'. Bucket: {BucketName}, Key: {ObjectKey}",
                    args.FileId, uploadId, containerName, objectKey);
                AbortMultipartUploadSilently(ossClient, containerName, objectKey, uploadId);
                throw;
            }
        }

        protected virtual void AbortMultipartUploadSilently(
            IOss ossClient,
            string containerName,
            string objectKey,
            string uploadId)
        {
            try
            {
                ossClient.AbortMultipartUpload(new AbortMultipartUploadRequest(containerName, objectKey, uploadId));
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to abort multipart upload '{UploadId}'. Bucket: {BucketName}, Key: {ObjectKey}", uploadId, containerName, objectKey);
            }
        }


        protected class OssClientLease
        {
            public IOss Client { get; }

            protected bool DisposeClient { get; }

            public OssClientLease(IOss client, bool disposeClient)
            {
                Client = client;
                DisposeClient = disposeClient;
            }

            public virtual void Return()
            {
                if (DisposeClient && Client is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KS3;
using KS3.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.KS3
{
    [ExposeKeyedService<IFileProvider>(KS3FileProviderConfigurationNames.ProviderName)]
    public class KS3FileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IClock Clock { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IKS3FileNameCalculator KS3FileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public KS3FileProvider(
            ILogger<KS3FileProvider> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IClock clock,
            IPoolOrchestrator poolOrchestrator,
            IKS3FileNameCalculator ks3FileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceScopeFactory = serviceScopeFactory;
            Options = options.Value;
            Clock = clock;
            PoolOrchestrator = poolOrchestrator;

            KS3FileNameCalculator = ks3FileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => KS3FileProviderConfigurationNames.ProviderName;

        protected virtual IObjectPool<IKS3> GetKS3ClientPool(KS3FileProviderConfiguration ks3Configuration)
        {
            var poolName = NormalizePoolName(ks3Configuration);
            var pool = PoolOrchestrator.GetObjectPool<IKS3, KS3ClientPolicy>(
                poolName,
                () => new KS3ClientPolicy(ServiceScopeFactory, ks3Configuration),
                Options.DefaultClientMaximumRetained);
            return pool;
        }

        protected virtual string NormalizePoolName(KS3FileProviderConfiguration ks3Configuration)
        {
            var v = $"{ks3Configuration.Endpoint}-{ks3Configuration.Protocol}-{ks3Configuration.UserAgent}-{ks3Configuration.MaxConnections}-{ks3Configuration.Timeout}-{ks3Configuration.ReadWriteTimeout}-{ks3Configuration.AccessKey}-{ks3Configuration.SecretKey}";
            using var sha1 = SHA1.Create();
            var hashBuffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(v));
            var hash = hashBuffer.Aggregate("", (current, b) => current + b.ToString("X2"));
            return $"FileStoring-{KS3FileProviderConfigurationNames.ProviderName}-{hash}";
        }


        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = KS3FileNameCalculator.Calculate(args);

            var ks3Configuration = args.Configuration.GetKS3Configuration();
            var pool = GetKS3ClientPool(ks3Configuration);
            var ks3Client = pool.Get();

            try
            {

                if (!args.OverrideExisting && FileExists(ks3Client, containerName, objectKey))
                {
                    throw new FileAlreadyExistsException($"Saving FILE '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }
                if (ks3Configuration.CreateContainerIfNotExists)
                {
                    if (!BucketExist(ks3Client, containerName))
                    {
                        ks3Client.CreateBucket(containerName);
                    }
                }

                if (args.Configuration.EnableAutoMultiPartUpload && args.FileStream!.Length > args.Configuration.MultiPartUploadMinFileSize)
                {
                    return await MultiPartUploadAsync(ks3Client, containerName, objectKey, args);
                }
                else
                {
                    return await PutObjectAsync(ks3Client, containerName, objectKey, args);
                }
            }
            finally
            {
                pool.Return(ks3Client);
            }
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = KS3FileNameCalculator.Calculate(args);

            var ks3Configuration = args.Configuration.GetKS3Configuration();
            var pool = GetKS3ClientPool(ks3Configuration);
            var ks3Client = pool.Get();

            try
            {

                if (!FileExists(ks3Client, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in KS3 when deleting. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return Task.FromResult(false);
                }
                ks3Client.DeleteObject(containerName, objectKey);
                return Task.FromResult(true);
            }
            finally
            {
                pool.Return(ks3Client);
            }
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = KS3FileNameCalculator.Calculate(args);

            var ks3Configuration = args.Configuration.GetKS3Configuration();
            var pool = GetKS3ClientPool(ks3Configuration);
            var ks3Client = pool.Get();

            try
            {

                return Task.FromResult(FileExists(ks3Client, containerName, objectKey));
            }
            finally
            {
                pool.Return(ks3Client);
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = KS3FileNameCalculator.Calculate(args);

            var ks3Configuration = args.Configuration.GetKS3Configuration();
            var pool = GetKS3ClientPool(ks3Configuration);
            var ks3Client = pool.Get();

            try
            {

                if (!FileExists(ks3Client, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in KS3. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return null;
                }
                var result = ks3Client.GetObject(containerName, objectKey);

                return await TryCopyToMemoryStreamAsync(result.ObjectContent, args.CancellationToken);
            }
            finally
            {
                pool.Return(ks3Client);
            }
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = KS3FileNameCalculator.Calculate(args);

            var ks3Configuration = args.Configuration.GetKS3Configuration();
            var pool = GetKS3ClientPool(ks3Configuration);
            var ks3Client = pool.Get();

            try
            {

                if (!FileExists(ks3Client, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in KS3. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return false;
                }

                var result = ks3Client.GetObject(containerName, objectKey);
                await TryWriteToFileAsync(result.ObjectContent, args.Path, args.CancellationToken);
                return true;
            }
            finally
            {
                pool.Return(ks3Client);
            }
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return Task.FromResult(string.Empty);
            }

            var containerName = GetContainerName(args);
            var objectKey = KS3FileNameCalculator.Calculate(args);

            var ks3Configuration = args.Configuration.GetKS3Configuration();
            var pool = GetKS3ClientPool(ks3Configuration);
            var ks3Client = pool.Get();

            try
            {

                if (args.CheckFileExist && !FileExists(ks3Client, containerName, objectKey))
                {
                    return Task.FromResult(string.Empty);
                }

                var datetime = args.Expires ?? Clock.Now.AddSeconds(600);
                var uri = ks3Client.GeneratePresignedUrl(containerName, objectKey, datetime);

                return Task.FromResult(uri.ToString());
            }
            finally
            {
                pool.Return(ks3Client);
            }
        }

        protected virtual string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetKS3Configuration();
            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.BucketName);
        }


        protected virtual bool BucketExist(IKS3 ks3, string containerName)
        {
            var headBucket = ks3.HeadBucket(containerName);
            if (headBucket.StatueCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        protected virtual bool FileExists(IKS3 ks3, string containerName, string fileName)
        {
            var headBucket = ks3.HeadBucket(containerName);
            if (headBucket.StatueCode == HttpStatusCode.OK)
            {
                try
                {
                    var headObject = ks3.HeadObject(containerName, fileName);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to check existence of object '{FileName}' in KS3 bucket '{ContainerName}'. Error: {ErrorMessage}", 
                        fileName, containerName, ex.Message);
                    return false;
                }
            }
            return false;
        }

        protected virtual Task<string> PutObjectAsync(
            IKS3 ks3,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            ks3.PutObject(new PutObjectRequest(containerName, objectKey, args.FileStream, new ObjectMetadata()));
            return Task.FromResult(args.FileId);
        }


        protected virtual async Task<string> MultiPartUploadAsync(
            IKS3 ks3,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var initiateMultipartUploadResult = ks3.InitiateMultipartUpload(new InitiateMultipartUploadRequest(containerName, objectKey));
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

                // Start multipart upload. partETags is a list that stores partETag. After KS3 receives the shard list submitted by the user,
                // it will verify the validity of each shard data one by one. After all data shards pass the verification,
                // KS3 will combine these shards into a complete file.
                var partETags = new List<PartETag>();
                for (var i = 0; i < partCount; i++)
                {
                    var skipBytes = (long)partSize * i;
                    // Position to the starting point of this upload.
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
                    var request = new UploadPartRequest()
                    {
                        BucketName = containerName,
                        ObjectKey = objectKey,
                        UploadId = uploadId,
                        InputStream = partStream,
                        PartNumber = i + 1
                    };

                    // Call the UploadPart interface to perform the upload function. The result contains the ETag value of this data shard.
                    var partETag = ks3.UploadPart(request);
                    partETags.Add(partETag);

                    Logger.LogDebug("Multipart upload progress for file '{FileId}' with UploadId '{UploadId}': {CompletedParts}/{TotalParts} completed.",
                        args.FileId, uploadId, partETags.Count, partCount);
                }

                var root = new XElement("CompleteMultipartUpload");
                foreach (var partETag in partETags)
                {
                    var partE = new XElement("Part");
                    partE.Add(new XElement("PartNumber", partETag.PartNumber));
                    partE.Add(new XElement("ETag", partETag.ETag));
                    root.Add(partE);
                }

                var contentBuffer = Encoding.UTF8.GetBytes(root.ToString());

                using var contentStream = new MemoryStream(contentBuffer);
                var completeMultipartUploadRequest = new CompleteMultipartUploadRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey,
                    UploadId = uploadId,
                    Content = contentStream
                };

                var completeMultipartUploadResult = ks3.CompleteMultipartUpload(completeMultipartUploadRequest);
                Logger.LogInformation("Multipart upload completed for file '{FileId}' with key '{ObjectKey}'. ETag: {ETag}",
                    args.FileId, completeMultipartUploadResult.Key, completeMultipartUploadResult.ETag);
                return args.FileId;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Multipart upload failed for file '{FileId}' with UploadId '{UploadId}'. Bucket: {BucketName}, Key: {ObjectKey}",
                    args.FileId, uploadId, containerName, objectKey);
                AbortMultipartUploadSilently(ks3, containerName, objectKey, uploadId);
                throw;
            }
        }
 
        protected virtual void AbortMultipartUploadSilently(
            IKS3 ks3,
            string containerName,
            string objectKey,
            string uploadId)
        {
            try
            {
                ks3.AbortMultipartUpload(new AbortMultipartUploadRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey,
                    UploadId = uploadId
                });
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to abort multipart upload '{UploadId}'. Bucket: {BucketName}, Key: {ObjectKey}", uploadId, containerName, objectKey);
            }
        }
 
    }
}

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
using Microsoft.Extensions.ObjectPool;
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
        protected IServiceProvider ServiceProvider { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }
        protected IClock Clock { get; }
        protected IPoolOrchestrator PoolOrchestrator { get; }
        protected IKS3FileNameCalculator KS3FileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public KS3FileProvider(
            ILogger<KS3FileProvider> logger,
            IServiceProvider serviceProvider,
            IOptions<AbpFileStoringAbstractionsOptions> options,
            IClock clock,
            IPoolOrchestrator poolOrchestrator,
            IKS3FileNameCalculator ks3FileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Options = options.Value;
            Clock = clock;
            PoolOrchestrator = poolOrchestrator;

            KS3FileNameCalculator = ks3FileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => KS3FileProviderConfigurationNames.ProviderName;

        protected virtual ObjectPool<IKS3> GetKS3ClientPool(KS3FileProviderConfiguration ks3Configuration)
        {
            var poolName = NormalizePoolName(ks3Configuration);
            var ks3ClientPolicy = ActivatorUtilities.CreateInstance<KS3ClientPolicy>(ServiceProvider, ks3Configuration);
            var pool = PoolOrchestrator.GetPool(poolName, ks3ClientPolicy, Options.DefaultClientMaximumRetained);
            return pool;
        }

        protected virtual string NormalizePoolName(KS3FileProviderConfiguration ks3Configuration)
        {
            var v = $"{ks3Configuration.Endpoint}-{ks3Configuration.AccessKey}-{ks3Configuration.SecretKey}";
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
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "KS3 HeadObject failed,{0}", ex.Message);
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

            //上传Id
            var uploadId = initiateMultipartUploadResult.UploadId;
            // 计算分片总数。
            var partSize = args.Configuration.MultiPartUploadShardingSize;
            //var fi = new FileInfo(spoolFile.FilePath);//?
            var fileSize = args.FileStream!.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            // 开始分片上传。partETags是保存partETag的列表，OSS收到用户提交的分片列表后，会逐一验证每个分片数据的有效性。 当所有的数据分片通过验证后，OSS会将这些分片组合成一个完整的文件。
            var partETags = new List<PartETag>();
            for (var i = 0; i < partCount; i++)
            {
                var skipBytes = (long)partSize * i;
                // 定位到本次上传的起始位置。
                args.FileStream.Seek(skipBytes, 0);

                // 计算本次上传的分片大小，最后一片为剩余的数据大小。
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var buffer = new byte[size];
                await args.FileStream.ReadAsync(buffer, 0, buffer.Length);

                var request = new UploadPartRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey,
                    UploadId = uploadId,
                    InputStream = new MemoryStream(buffer),
                    PartNumber = i + 1
                };

                // 调用UploadPart接口执行上传功能，返回结果中包含了这个数据片的ETag值。
                var partETag = ks3.UploadPart(request);
                partETags.Add(partETag);

                Logger.LogDebug("UploadId {uploadId} finish {Count}/{partCount}.", uploadId, partETags.Count, partCount);
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

            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest()
            {
                BucketName = containerName,
                ObjectKey = objectKey,
                UploadId = uploadId,
                Content = new MemoryStream(contentBuffer)
            };

            var completeMultipartUploadResult = ks3.CompleteMultipartUpload(completeMultipartUploadRequest);
            Logger.LogDebug("CompleteMultipartUpload {Key} ({ETag}).", completeMultipartUploadResult.Key, completeMultipartUploadResult.ETag);
            return args.FileId;
        }
 

    }
}

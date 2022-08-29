using Aliyun.OSS;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public class AliyunFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IOssClientFactory OssClientFactory { get; }
        protected IAliyunFileNameCalculator AliyunFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public AliyunFileProvider(
            ILogger<AliyunFileProvider> logger,
            IClock clock,
            IOssClientFactory ossClientFactory,
            IAliyunFileNameCalculator aliyunFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            Clock = clock;
            OssClientFactory = ossClientFactory;
            AliyunFileNameCalculator = aliyunFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => AliyunFileProviderConfigurationNames.ProviderName;

        protected virtual IOss GetOssClient(FileContainerConfiguration fileContainerConfiguration)
        {
            var aliyunConfiguration = fileContainerConfiguration.GetAliyunConfiguration();
            return OssClientFactory.Create(aliyunConfiguration);
        }

        protected virtual IOss GetOssClient(AliyunFileProviderConfiguration aliyunConfiguration)
        {
            return OssClientFactory.Create(aliyunConfiguration);
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var aliyunConfig = args.Configuration.GetAliyunConfiguration();
            var ossClient = GetOssClient(aliyunConfig);
            if (!args.OverrideExisting && FileExistsAsync(ossClient, containerName, objectKey))
            {
                throw new FileAlreadyExistsException($"Saving FILE '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }
            if (aliyunConfig.CreateContainerIfNotExists)
            {
                if (!ossClient.DoesBucketExist(containerName))
                {
                    ossClient.CreateBucket(containerName);
                }
            }

            if (args.Configuration.EnableAutoMultiPartUpload && args.FileStream.Length > args.Configuration.MultiPartUploadMinFileSize)
            {
                return await MultiPartUploadAsync(ossClient, containerName, objectKey, args);
            }
            else
            {
                return await PutObjectAsync(ossClient, containerName, objectKey, args);
            }
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            if (!FileExistsAsync(ossClient, containerName, objectKey))
            {
                return Task.FromResult(false);
            }
            ossClient.DeleteObject(containerName, objectKey);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            return Task.FromResult(FileExistsAsync(ossClient, containerName, objectKey));
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            if (!FileExistsAsync(ossClient, containerName, objectKey))
            {
                return null;
            }
            var result = ossClient.GetObject(containerName, objectKey);

            return await TryCopyToMemoryStreamAsync(result.Content, args.CancellationToken);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            if (!FileExistsAsync(ossClient, containerName, objectKey))
            {
                return false;
            }

            var result = ossClient.GetObject(containerName, objectKey);
            await TryWriteToFileAsync(result.Content, args.Path, args.CancellationToken);
            return true;
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return Task.FromResult(string.Empty);
            }

            var containerName = GetContainerName(args);
            var objectKey = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);

            if (args.CheckFileExist && !FileExistsAsync(ossClient, containerName, objectKey))
            {
                return Task.FromResult(string.Empty);
            }

            var datetime = args.Expires ?? Clock.Now.AddSeconds(600);
            var uri = ossClient.GeneratePresignedUri(containerName, objectKey, datetime);

            return Task.FromResult(uri.ToString());
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

            //上传Id
            var uploadId = initiateMultipartUploadResult.UploadId;
            // 计算分片总数。
            var partSize = args.Configuration.MultiPartUploadShardingSize;
            //var fi = new FileInfo(spoolFile.FilePath);//?
            var fileSize = args.FileStream.Length;
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
                args.FileStream.Seek(skipBytes, 0);

                // 计算本次上传的分片大小，最后一片为剩余的数据大小。
                var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                var buffer = new byte[size];
                await args.FileStream.ReadAsync(buffer, 0, buffer.Length);

                var request = new UploadPartRequest(containerName, objectKey, uploadId)
                {
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1
                };
                // 调用UploadPart接口执行上传功能，返回结果中包含了这个数据片的ETag值。
                var result = ossClient.UploadPart(request);
                partETags.Add(result.PartETag);

                Logger.LogDebug("UploadId {uploadId} finish {Count}/{partCount}.", uploadId, partETags.Count, partCount);
            }
            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(containerName, objectKey, uploadId);
            foreach (var partETag in partETags)
            {
                completeMultipartUploadRequest.PartETags.Add(partETag);
            }

            var completeMultipartUploadResult = ossClient.CompleteMultipartUpload(completeMultipartUploadRequest);
            Logger.LogDebug("CompleteMultipartUpload {Key} ({ETag}).", completeMultipartUploadResult.Key, completeMultipartUploadResult.ETag);
            return args.FileId;
        }

    }
}

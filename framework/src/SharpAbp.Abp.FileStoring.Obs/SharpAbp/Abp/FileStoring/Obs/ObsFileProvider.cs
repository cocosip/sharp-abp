using Microsoft.Extensions.Logging;
using OBS;
using OBS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Obs
{
    [ExposeKeyedService<IFileProvider>(ObsFileProviderConfigurationNames.ProviderName)]
    public class ObsFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IObsClientFactory ObsClientFactory { get; }
        protected IObsFileNameCalculator ObsFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public ObsFileProvider(
            ILogger<ObsFileProvider> logger,
            IClock clock,
            IObsClientFactory obsClientFactory,
            IObsFileNameCalculator obsFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            Clock = clock;
            ObsClientFactory = obsClientFactory;
            ObsFileNameCalculator = obsFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => ObsFileProviderConfigurationNames.ProviderName;

        protected virtual ObsClient GetObsClient(FileContainerConfiguration fileContainerConfiguration)
        {
            var obsConfiguration = fileContainerConfiguration.GetObsConfiguration();
            return ObsClientFactory.Create(obsConfiguration);
        }

        protected virtual ObsClient GetObsClient(ObsFileProviderConfiguration obsConfiguration)
        {
            return ObsClientFactory.Create(obsConfiguration);
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);
            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var obsClient = GetObsClient(obsConfiguration);
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

            if (args.Configuration.EnableAutoMultiPartUpload && args.FileStream.Length > args.Configuration.MultiPartUploadMinFileSize)
            {
                return await MultiPartUploadAsync(obsClient, containerName, objectKey, args);
            }
            else
            {
                return await PutObjectAsync(obsClient, containerName, objectKey, args);
            }
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);
            var obsClient = GetObsClient(args.Configuration);
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

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);
            var obsClient = GetObsClient(args.Configuration);
            return Task.FromResult(FileExistsAsync(obsClient, containerName, objectKey));
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);
            var obsClient = GetObsClient(args.Configuration);
            if (!FileExistsAsync(obsClient, containerName, objectKey))
            {
                return null;
            }
            var result = obsClient.GetObject(new GetObjectRequest() { BucketName = containerName, ObjectKey = objectKey });

            return await TryCopyToMemoryStreamAsync(result.OutputStream, args.CancellationToken);
        }


        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);
            var obsClient = GetObsClient(args.Configuration);
            if (!FileExistsAsync(obsClient, containerName, objectKey))
            {
                return false;
            }

            var result = obsClient.GetObject(new GetObjectRequest() { BucketName = containerName, ObjectKey = objectKey });
            await TryWriteToFileAsync(result.OutputStream, args.Path, args.CancellationToken);
            return true;
        }


        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return Task.FromResult(string.Empty);
            }

            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);
            var obsClient = GetObsClient(args.Configuration);

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

            //上传Id
            var uploadId = initiateMultipartUploadResponse.UploadId;
            // 计算分片总数。
            var partSize = args.Configuration.MultiPartUploadShardingSize;

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

                var request = new UploadPartRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey,
                    UploadId = uploadId,
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1
                };
                // 调用UploadPart接口执行上传功能，返回结果中包含了这个数据片的ETag值。
                var result = obsClient.UploadPart(request);
                partETags.Add(new PartETag(result.PartNumber, result.ETag));

                Logger.LogDebug("UploadId {uploadId} finish {Count}/{partCount}.", uploadId, partETags.Count, partCount);
            }

            var completeMultipartUploadResponse = obsClient.CompleteMultipartUpload(new CompleteMultipartUploadRequest()
            {
                BucketName = containerName,
                ObjectKey = objectKey,
                UploadId = uploadId,
                PartETags = partETags
            });

            Logger.LogDebug("CompleteMultipartUpload {ObjectKey} ({ETag}).", completeMultipartUploadResponse.ObjectKey, completeMultipartUploadResponse.ETag);
            return args.FileId;
        }


    }
}

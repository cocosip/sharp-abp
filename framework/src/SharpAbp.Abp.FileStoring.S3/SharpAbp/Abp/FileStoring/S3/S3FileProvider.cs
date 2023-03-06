using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3FileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IS3FileNameCalculator FileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        protected IS3ClientFactory ClientFactory { get; }

        public S3FileProvider(
            ILogger<S3FileProvider> logger,
            IClock clock,
            IS3FileNameCalculator fileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService,
            IS3ClientFactory clientFactory)
        {
            Logger = logger;
            Clock = clock;
            FileNameCalculator = fileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
            ClientFactory = clientFactory;
        }

        public override string Provider => S3FileProviderConfigurationNames.ProviderName;

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetS3Configuration();
            var client = GetS3Client(args);


            if (!args.OverrideExisting && await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateBucketIfNotExists)
            {
                await CreateBucketIfNotExists(client, containerName);
            }

            string fileId;
            if (args.Configuration.EnableAutoMultiPartUpload && (args.FileStream.Length > args.Configuration.MultiPartUploadMinFileSize))
            {
                fileId = await MultipartUploadAsync(client, containerName, objectKey, configuration, args);
            }
            else
            {
                fileId = await SingleUploadAsync(client, containerName, objectKey, configuration, args);
            }

            args.FileStream?.Dispose();

            return fileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);

            if (await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                await client.DeleteAsync(containerName, objectKey, null, args.CancellationToken);
                return true;
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);

            return await FileExistsAsync(client, containerName, objectKey, args.CancellationToken);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);

            var getObjectResponse = await client.GetObjectAsync(containerName, objectKey);
            await getObjectResponse.WriteResponseStreamToFileAsync(args.Path, true, args.CancellationToken);
            return true;
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);

            if (!await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                return null;
            }

            var getObjectResponse = await client.GetObjectAsync(containerName, objectKey);
            if (getObjectResponse.ResponseStream != null)
            {
                var memoryStream = new MemoryStream();
                await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
                return memoryStream;
            }
            return null;
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var containerName = GetContainerName(args);
            var objectKey = FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);

            var configuration = args.Configuration.GetS3Configuration();

            if (args.CheckFileExist && !await FileExistsAsync(client, containerName, objectKey, args.CancellationToken))
            {
                return string.Empty;
            }

            var preSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = containerName,
                Key = objectKey,
                Protocol = (Protocol)configuration.Protocol,
                Expires = args.Expires ?? Clock.Now.AddSeconds(600)
            };

            var accessUrl = client.GetPreSignedURL(preSignedUrlRequest);

            return accessUrl;
        }


        /// <summary>单文件上传
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
            //UploadId
            var uploadId = initiateMultipartUploadResponse.UploadId;
            //Calculate slice part count
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
                // 计算本次上传的片大小，最后一片为剩余的数据大小。
                var size = (int)((partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes));

                byte[] buffer = new byte[size];
                args.FileStream.Read(buffer, 0, size);

                //分片上传
                var uploadPartResponse = await client.UploadPartAsync(new UploadPartRequest()
                {
                    BucketName = containerName,
                    UploadId = uploadId,
                    Key = objectKey,
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1,
                    //UseChunkEncoding = useChunkEncoding
                });
                partETags.Add(new PartETag(uploadPartResponse.PartNumber, uploadPartResponse.ETag));
                Logger.LogDebug("Upload part file ,key:{0},UploadId:{1},Complete {2}/{3}", objectKey, uploadId, partETags.Count, partCount);
            }

            //完成上传分片
            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest()
            {
                BucketName = containerName,
                Key = objectKey,
                UploadId = uploadId,
                PartETags = partETags
            };

            var completeMultipartUploadResponse = await client.CompleteMultipartUploadAsync(completeMultipartUploadRequest, args.CancellationToken);
            Logger.LogDebug("CompleteMultipartUpload {Key} ({ETag}).", completeMultipartUploadResponse.Key, completeMultipartUploadResponse.ETag);
            return args.FileId;
        }


        protected virtual IAmazonS3 GetS3Client(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetS3Configuration();
            var client = ClientFactory.Create(configuration);
            return client;
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

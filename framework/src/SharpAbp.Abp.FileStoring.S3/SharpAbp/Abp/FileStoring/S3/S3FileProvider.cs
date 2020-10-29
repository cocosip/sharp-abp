using Amazon.S3;
using Amazon.S3.Model;
using AmazonKS3;
using AutoS3;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3FileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IS3FileNameCalculator S3FileNameCalculator { get; }
        protected IS3ClientFactory S3ClientFactory { get; }

        public S3FileProvider(ILogger<S3FileProvider> logger, IS3FileNameCalculator s3FileNameCalculator, IS3ClientFactory s3ClientFactory)
        {
            Logger = logger;
            S3FileNameCalculator = s3FileNameCalculator;
            S3ClientFactory = s3ClientFactory;
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetS3Configuration();
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            if (!args.OverrideExisting && await FileExistsAsync(client, containerName, fileName, args.CancellationToken))
            {
                throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateBucketIfNotExists)
            {
                await CreateBucketIfNotExists(client, containerName);
            }

            if (configuration.EnableSlice && (args.FileStream.Length > configuration.SliceSize))
            {
                await MultipartUploadInternal(client, containerName, fileName, args.FileStream, configuration.UseChunkEncoding, configuration.SliceSize);
            }
            else
            {
                await SingleUploadInternal(client, containerName, fileName, args.FileStream, configuration.UseChunkEncoding);
            }

            args.FileStream?.Dispose();

            return fileName;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            if (await FileExistsAsync(client, containerName, fileName, args.CancellationToken))
            {
                await client.DeleteAsync(containerName, fileName, null, args.CancellationToken);
                return true;
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            return await FileExistsAsync(client, containerName, fileName, args.CancellationToken);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            var getObjectResponse = await client.GetObjectAsync(containerName, fileName);
            await getObjectResponse.WriteResponseStreamToFileAsync(args.Path, true, args.CancellationToken);
            return true;
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            if (!await FileExistsAsync(client, containerName, fileName, args.CancellationToken))
            {
                return null;
            }

            var getObjectResponse = await client.GetObjectAsync(containerName, fileName);
            if (getObjectResponse.ResponseStream != null)
            {
                var memoryStream = new MemoryStream();
                await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
                return memoryStream;
            }
            return null;
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            var configuration = args.Configuration.GetS3Configuration();

            var preSignedUrlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = containerName,
                Key = fileName,
                Protocol = (Protocol)configuration.Protocol
            };

            if (args.Expires.HasValue)
            {
                preSignedUrlRequest.Expires = args.Expires.Value;
            }

            var accessUrl = client.GetPreSignedURL(preSignedUrlRequest);

            return Task.FromResult(accessUrl);
        }


        /// <summary>单文件上传
        /// </summary>
        private async Task<PutObjectResponse> SingleUploadInternal(IAmazonS3 client, string bucketName, string fileName, Stream stream, bool useChunkEncoding)
        {
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = stream,
                AutoCloseStream = true,
                UseChunkEncoding = useChunkEncoding
            };
            var putObjectResponse = await client.PutObjectAsync(putObjectRequest);
            return putObjectResponse;
        }

        private async Task<CompleteMultipartUploadResponse> MultipartUploadInternal(IAmazonS3 client, string bucketName, string fileName, Stream stream, bool useChunkEncoding, int sliceSize)
        {
            var initiateMultipartUploadResponse = await client.InitiateMultipartUploadAsync(bucketName, fileName);
            //UploadId
            var uploadId = initiateMultipartUploadResponse.UploadId;
            //Calculate slice part count
            var partSize = sliceSize;
            //var fi = new FileInfo(spoolFile.FilePath);//?
            var fileSize = stream.Length;
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
                stream.Read(buffer, 0, size);

                //分片上传
                var uploadPartResponse = await client.UploadPartAsync(new UploadPartRequest()
                {
                    BucketName = bucketName,
                    UploadId = uploadId,
                    Key = fileName,
                    InputStream = new MemoryStream(buffer),
                    PartSize = size,
                    PartNumber = i + 1,
                    UseChunkEncoding = useChunkEncoding
                });
                partETags.Add(new PartETag(uploadPartResponse.PartNumber, uploadPartResponse.ETag));
                Logger.LogDebug("Upload part file ,key:{0},UploadId:{1},Complete {2}/{3}", fileName, uploadId, partETags.Count, partCount);
            }


            //完成上传分片
            var completeMultipartUploadResponse = await client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = fileName,
                UploadId = uploadId,
                PartETags = partETags
            });

            return completeMultipartUploadResponse;
        }



        protected virtual IAmazonS3 GetS3Client(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetS3Configuration();

            var amazonS3 = S3ClientFactory.GetOrAddClient(configuration.AccessKeyId, configuration.SecretAccessKey, () =>
             {
                 var s3ClientConfiguration = new S3ClientConfiguration()
                 {
                     Vendor = (S3VendorType)configuration.VendorType,
                     AccessKeyId = configuration.AccessKeyId,
                     SecretAccessKey = configuration.SecretAccessKey,
                     MaxClient = configuration.MaxClient,
                 };

                 if (configuration.VendorType == (int)S3VendorType.KS3)
                 {
                     s3ClientConfiguration.Config = new AmazonS3Config()
                     {
                         ServiceURL = configuration.ServerUrl,
                         ForcePathStyle = configuration.ForcePathStyle,
                         SignatureVersion = configuration.SignatureVersion
                     };
                 }
                 else
                 {
                     s3ClientConfiguration.Config = new AmazonKS3Config()
                     {
                         ServiceURL = configuration.ServerUrl,
                         ForcePathStyle = configuration.ForcePathStyle,
                         SignatureVersion = configuration.SignatureVersion
                     };
                 }

                 return s3ClientConfiguration;
             });

            return amazonS3;
        }

        protected virtual async Task CreateBucketIfNotExists(IAmazonS3 client, string containerName)
        {
            await client.EnsureBucketExistsAsync(containerName);
        }

        private async Task<bool> FileExistsAsync(IAmazonS3 client, string containerName, string fileId, CancellationToken cancellationToken)
        {
            // Make sure Blob Container exists.
            if (await client.DoesS3BucketExistAsync(containerName))
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

        private static string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetS3Configuration();

            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : configuration.BucketName;
        }
    }
}

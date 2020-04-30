using Amazon.S3;
using Amazon.S3.Model;
using DotCommon.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.FileSystem.S3
{
    /// <summary>S3文件存储客户端
    /// </summary>
    public class S3FileStoreClient : IFileStoreClient
    {
        private readonly ILogger _logger;
        private readonly IS3ClientFactory _s3ClientFactory;
        private readonly IFileIdGenerator _fileIdGenerator;
        private readonly FileSystemOption _option;
        public S3FileStoreClient(ILogger<S3FileStoreClient> logger, IS3ClientFactory s3ClientFactory, IFileIdGenerator fileIdGenerator, IOptions<FileSystemOption> option)
        {
            _logger = logger;
            _s3ClientFactory = s3ClientFactory;
            _fileIdGenerator = fileIdGenerator;
            _option = option.Value;
        }

        /// <summary>上传文件
        /// </summary>
        public async Task<FileIdentifier> UploadFileAsync(Patch patch, UploadFileInfo info)
        {
            var s3Patch = (S3Patch)patch;
            var isMultipartUpload = IsMultipartUpload(s3Patch.Bucket, info);

            if (isMultipartUpload)
            {
                return await MultipartUploadFile(s3Patch, info);
            }
            else
            {
                return await SingleUploadFile(s3Patch, info);
            }
        }

        /// <summary>删除文件
        /// </summary>
        public async Task DeleteFileAsync(FileIdentifier fileIdentifier)
        {
            var s3Patch = (S3Patch)fileIdentifier.Patch;
            var client = _s3ClientFactory.GetClientByBucket(s3Patch.Bucket);

            var deleteObjectRequest = new DeleteObjectRequest()
            {
                BucketName = s3Patch.Bucket.Name,
                Key = fileIdentifier.FileId
            };

            await client.DeleteObjectAsync(deleteObjectRequest);
        }

        /// <summary>下载文件到指定目录
        /// </summary>
        public async Task DownloadFileAsync(FileIdentifier fileIdentifier, string savePath)
        {
            var s3Patch = (S3Patch)fileIdentifier.Patch;
            var client = _s3ClientFactory.GetClientByBucket(s3Patch.Bucket);
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = s3Patch.Bucket.Name,
                Key = fileIdentifier.FileId
            };
            var getObjectResponse = await client.GetObjectAsync(getObjectRequest);
            await getObjectResponse.WriteResponseStreamToFileAsync(savePath, true, CancellationToken.None);
        }

        /// <summary>获取文件二进制
        /// </summary>
        public async Task<byte[]> GetFileAsync(FileIdentifier fileIdentifier)
        {
            var s3Patch = (S3Patch)fileIdentifier.Patch;
            var client = _s3ClientFactory.GetClientByBucket(s3Patch.Bucket);
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = s3Patch.Bucket.Name,
                Key = fileIdentifier.FileId
            };
            var getObjectResponse = await client.GetObjectAsync(getObjectRequest);
            if (getObjectResponse.ResponseStream.Length > 1024 * 1024 * 100)
            {
                throw new OutOfMemoryException($"下载文件过大,请保存到磁盘,文件大小:{getObjectResponse.ContentLength}");
            }

            var buffer = StreamUtil.StreamToBuffer(getObjectResponse.ResponseStream);
            return buffer;
        }

        /// <summary>获取文件数据
        /// </summary>
        public async Task<FileMetaInfo> GetMetaInfoAsync(FileIdentifier fileIdentifier)
        {
            var fileMeta = new FileMetaInfo()
            {
                FileId = fileIdentifier.FileId
            };

            var s3Patch = (S3Patch)fileIdentifier.Patch;
            var client = _s3ClientFactory.GetClientByBucket(s3Patch.Bucket);
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = s3Patch.Bucket.Name,
                Key = fileIdentifier.FileId
            };
            var getObjectResponse = await client.GetObjectAsync(getObjectRequest);
            //大小
            fileMeta.FileSize = getObjectResponse.ResponseStream.Length;
            getObjectResponse?.Dispose();

            return fileMeta;
        }

        /// <summary>获取文件的访问地址
        /// </summary>
        public string GetUrl(FileIdentifier fileIdentifier, DateTime? expires = null)
        {
            var s3Patch = (S3Patch)fileIdentifier.Patch;
            var client = _s3ClientFactory.GetClientByBucket(s3Patch.Bucket);

            var getPreSignedUrlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = s3Patch.Bucket.Name,
                Key = fileIdentifier.FileId,
                Protocol = GetProtocol((S3Protocol)s3Patch.Bucket.S3Protocol)
            };
            if (expires.HasValue)
            {
                getPreSignedUrlRequest.Expires = expires.Value;
            }

            return client.GetPreSignedURL(getPreSignedUrlRequest);
        }


        /// <summary>单文件上传
        /// </summary>
        private async Task<FileIdentifier> SingleUploadFile(S3Patch patch, UploadFileInfo info)
        {
            var fileId = _fileIdGenerator.GenerateFileId(StoreType.S3, patch, info);

            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = patch.Bucket.Name,
                AutoCloseStream = true,
                Key = fileId,
                CannedACL = GetS3CannedACL((S3Acl)patch.Bucket.Acl)
            };
            if (info.FileData != null)
            {
                putObjectRequest.InputStream = new MemoryStream(info.FileData);
            }
            else
            {
                putObjectRequest.FilePath = info.FilePath;
            }

            var client = _s3ClientFactory.GetClientByBucket(patch.Bucket);

            var putObjectResponse = await client.PutObjectAsync(putObjectRequest);

            var fileIdentifier = new FileIdentifier()
            {
                FileId = fileId,
                Patch = patch,
                StoreType = StoreType.S3
            };
            return fileIdentifier;
        }


        /// <summary>分片上传数据
        /// </summary>
        private async Task<FileIdentifier> MultipartUploadFile(S3Patch patch, UploadFileInfo info)
        {
            var fileId = _fileIdGenerator.GenerateFileId(StoreType.S3, patch, info);

            //获取S3客户端
            var client = _s3ClientFactory.GetClientByBucket(patch.Bucket);

            //初始化分片上传

            var initiateMultipartUploadResponse = await client.InitiateMultipartUploadAsync(patch.Bucket.Name, fileId);
            //上传Id
            var uploadId = initiateMultipartUploadResponse.UploadId;
            // 计算分片总数。
            var partSize = patch.Bucket.SliceSize;
            //var fi = new FileInfo(spoolFile.FilePath);//?
            var fileSize = GetFileSize(info);
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            // 开始分片上传。partETags是保存partETag的列表，OSS收到用户提交的分片列表后，会逐一验证每个分片数据的有效性。 当所有的数据分片通过验证后，OSS会将这些分片组合成一个完整的文件。
            var partETags = new List<PartETag>();

            using (var fs = File.Open(info.FilePath, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    var skipBytes = (long)partSize * i;
                    // 计算本次上传的片大小，最后一片为剩余的数据大小。
                    var size = (int)((partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes));

                    byte[] buffer = new byte[size];
                    fs.Read(buffer, 0, size);

                    //分片上传
                    var uploadPartResponse = await client.UploadPartAsync(new UploadPartRequest()
                    {
                        BucketName = patch.Bucket.Name,
                        UploadId = uploadId,
                        Key = fileId,
                        InputStream = new MemoryStream(buffer),
                        PartSize = size,
                        PartNumber = i + 1
                    });
                    partETags.Add(new PartETag(uploadPartResponse.PartNumber, uploadPartResponse.ETag));
                    _logger.LogDebug("上传文件Key:{0},UploadId:{1},完成 {2}/{3}", fileId, uploadId, partETags.Count, partCount);
                }
            }

            //完成上传分片
            var completeMultipartUploadResponse = await client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest()
            {
                BucketName = patch.Bucket.Name,
                Key = fileId,
                UploadId = uploadId,
                PartETags = partETags
            });

            _logger.LogDebug("分片上传完成,Key:{0},Tags:[{1}]", completeMultipartUploadResponse.Key, string.Join(",", partETags));


            var fileIdentifier = new FileIdentifier()
            {
                FileId = fileId,
                Patch = patch,
                StoreType = StoreType.S3
            };
            return fileIdentifier;
        }



        /// <summary>是否分片上传
        /// </summary>
        private bool IsMultipartUpload(BucketInfo bucket, UploadFileInfo info)
        {
            var size = GetFileSize(info);
            //如果有文件流,则从流中获取数据大小
            return size > bucket.SliceSize;
        }

        /// <summary>获取文件大小
        /// </summary>
        private long GetFileSize(UploadFileInfo info)
        {
            var size = info.FileData != null ? info.FileData.Length : new FileInfo(info.FilePath).Length;
            return size;
        }

        private S3CannedACL GetS3CannedACL(S3Acl s3Acl)
        {
            switch (s3Acl)
            {
                case S3Acl.Private:
                    return S3CannedACL.Private;
                case S3Acl.PublicRead:
                    return S3CannedACL.PublicRead;
                case S3Acl.PublicReadWrite:
                    return S3CannedACL.PublicReadWrite;
                case S3Acl.AuthenticatedRead:
                    return S3CannedACL.AuthenticatedRead;
                default:
                    return S3CannedACL.NoACL;
            }
        }


        private Protocol GetProtocol(S3Protocol s3Protocol)
        {
            switch (s3Protocol)
            {
                case S3Protocol.HTTPS:
                    return Protocol.HTTPS;
                case S3Protocol.HTTP:
                    return Protocol.HTTP;
                default:
                    return Protocol.HTTP;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OBS;
using OBS.Model;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Obs
{
    [ExposeKeyedService<IFileProvider>(ObsFileProviderConfigurationNames.ProviderName)]
    public class ObsFileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected IClock Clock { get; }
        protected FileProviderClientCache<ObsClient> ClientCache { get; }
        protected IObsFileNameCalculator ObsFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public ObsFileProvider(
            ILogger<ObsFileProvider> logger,
            IServiceScopeFactory serviceScopeFactory,
            IClock clock,
            FileProviderClientCache<ObsClient> clientCache,
            IObsFileNameCalculator obsFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            ServiceScopeFactory = serviceScopeFactory;
            Clock = clock;
            ClientCache = clientCache;
            ObsFileNameCalculator = obsFileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => ObsFileProviderConfigurationNames.ProviderName;


        protected virtual ObsClientLease GetObsClientLease(ObsFileProviderConfiguration obsConfiguration)
        {
            var client = ClientCache.GetOrAdd(
                BuildClientKey(obsConfiguration),
                () => new ObsClientPolicy(ServiceScopeFactory, obsConfiguration).Create());

            return new ObsClientLease(client);
        }

        protected virtual string BuildClientKey(ObsFileProviderConfiguration obsConfiguration)
        {
            return FileProviderClientKey.Build(
                Provider,
                ("Endpoint", obsConfiguration.Endpoint),
                ("AccessKeyId", obsConfiguration.AccessKeyId),
                ("AccessKeySecret", obsConfiguration.AccessKeySecret));
        }


        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var lease = GetObsClientLease(obsConfiguration);
            var obsClient = lease.Client;

            try
            {
                if (!args.OverrideExisting && await FileExistsAsync(obsClient, containerName, objectKey))
                {
                    throw new FileAlreadyExistsException($"Saving FILE '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
                }
                if (obsConfiguration.CreateContainerIfNotExists)
                {
                    if (!await HeadBucketAsync(obsClient, containerName))
                    {
                        obsClient.CreateBucket(new CreateBucketRequest() { BucketName = containerName });
                    }
                }

                if (args.Configuration.EnableAutoMultiPartUpload && args.FileStream!.Length > args.Configuration.MultiPartUploadMinFileSize)
                {
                    return await MultiPartUploadAsync(obsClient, containerName, objectKey, args);
                }
                else
                {
                    return await PutObjectAsync(obsClient, containerName, objectKey, args);
                }
            }
            finally
            {
                lease.Return();
            }
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var lease = GetObsClientLease(obsConfiguration);
            var obsClient = lease.Client;
            try
            {

                if (!await FileExistsAsync(obsClient, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in OBS when deleting. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return false;
                }

                await DeleteObjectAsync(obsClient, containerName, objectKey);
                return true;
            }
            finally
            {
                lease.Return();
            }
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var lease = GetObsClientLease(obsConfiguration);
            var obsClient = lease.Client;

            try
            {

                return await FileExistsAsync(obsClient, containerName, objectKey);
            }
            finally
            {
                lease.Return();
            }
        }

        public override async Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var lease = GetObsClientLease(obsConfiguration);
            var obsClient = lease.Client;

            try
            {
                if (!await FileExistsAsync(obsClient, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in OBS. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return null;
                }
                using var result = await GetObjectAsync(obsClient, containerName, objectKey);

                return await TryCopyToMemoryStreamAsync(result.OutputStream, args.CancellationToken);
            }
            finally
            {
                lease.Return();
            }
        }


        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var lease = GetObsClientLease(obsConfiguration);
            var obsClient = lease.Client;

            try
            {
                if (!await FileExistsAsync(obsClient, containerName, objectKey))
                {
                    Logger.LogWarning("File not found in OBS. Container: {ContainerName}, ObjectKey: {ObjectKey}, FileId: {FileId}", containerName, objectKey, args.FileId);
                    return false;
                }

                using var result = await GetObjectAsync(obsClient, containerName, objectKey);
                await TryWriteToFileAsync(result.OutputStream, args.Path, args.CancellationToken);
                return true;
            }
            finally
            {
                lease.Return();
            }
        }


        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var containerName = GetContainerName(args);
            var objectKey = ObsFileNameCalculator.Calculate(args);

            var obsConfiguration = args.Configuration.GetObsConfiguration();
            var lease = GetObsClientLease(obsConfiguration);
            var obsClient = lease.Client;

            try
            {
                if (args.CheckFileExist && !await FileExistsAsync(obsClient, containerName, objectKey))
                {
                    return string.Empty;
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

                return temporarySignatureResponse.SignUrl;
            }
            finally
            {
                lease.Return();
            }
        }

        protected virtual string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetObsConfiguration();
            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.BucketName);
        }

        protected virtual async Task<bool> FileExistsAsync(ObsClient obsClient, string containerName, string fileName)
        {
            if (!await HeadBucketAsync(obsClient, containerName))
            {
                return false;
            }

            var hadObjectRequest = new HeadObjectRequest()
            {
                BucketName = containerName,
                ObjectKey = fileName
            };

            return await Task<bool>.Factory.FromAsync(
                (callback, state) => obsClient.BeginHeadObject(hadObjectRequest, callback, state),
                obsClient.EndHeadObject,
                null);
        }

        protected virtual Task<bool> HeadBucketAsync(ObsClient obsClient, string containerName)
        {
            var request = new HeadBucketRequest()
            {
                BucketName = containerName
            };

            return Task<bool>.Factory.FromAsync(
                (callback, state) => obsClient.BeginHeadBucket(request, callback, state),
                obsClient.EndHeadBucket,
                null);
        }

        protected virtual Task<GetObjectResponse> GetObjectAsync(ObsClient obsClient, string containerName, string objectKey)
        {
            var request = new GetObjectRequest()
            {
                BucketName = containerName,
                ObjectKey = objectKey
            };

            return Task<GetObjectResponse>.Factory.FromAsync(
                (callback, state) => obsClient.BeginGetObject(request, callback, state),
                obsClient.EndGetObject,
                null);
        }

        protected virtual Task DeleteObjectAsync(ObsClient obsClient, string containerName, string objectKey)
        {
            var request = new DeleteObjectRequest()
            {
                BucketName = containerName,
                ObjectKey = objectKey
            };

            return Task<DeleteObjectResponse>.Factory.FromAsync(
                (callback, state) => obsClient.BeginDeleteObject(request, callback, state),
                obsClient.EndDeleteObject,
                null);
        }

        protected virtual async Task<string> PutObjectAsync(
            ObsClient obsClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var request = new PutObjectRequest
            {
                BucketName = containerName,
                ObjectKey = objectKey,
                InputStream = args.FileStream,
            };

            await Task<PutObjectResponse>.Factory.FromAsync(
                (callback, state) => obsClient.BeginPutObject(request, callback, state),
                obsClient.EndPutObject,
                null);

            return args.FileId;
        }

        protected virtual async Task<string> MultiPartUploadAsync(
            ObsClient obsClient,
            string containerName,
            string objectKey,
            FileProviderSaveArgs args)
        {
            var initiateMultipartUploadResponse = await Task<InitiateMultipartUploadResponse>.Factory.FromAsync(
                (callback, state) => obsClient.BeginInitiateMultipartUpload(new InitiateMultipartUploadRequest()
                {
                    BucketName = containerName,
                    ObjectKey = objectKey
                }, callback, state),
                obsClient.EndInitiateMultipartUpload,
                null);

            var uploadId = initiateMultipartUploadResponse.UploadId;

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

                // Start multipart upload. partETags is a list that stores partETag. After OBS receives the shard list submitted by the user,
                // it will verify the validity of each shard data one by one. After all data shards pass the verification,
                // OBS will combine these shards into a complete file.
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
                    var request = new UploadPartRequest()
                    {
                        BucketName = containerName,
                        ObjectKey = objectKey,
                        UploadId = uploadId,
                        InputStream = partStream,
                        PartSize = size,
                        PartNumber = i + 1
                    };
                    // Call the UploadPart interface to perform the upload function. The result contains the ETag value of this data shard.
                    var result = await Task<UploadPartResponse>.Factory.FromAsync(
                        (callback, state) => obsClient.BeginUploadPart(request, callback, state),
                        obsClient.EndUploadPart,
                        null);
                    partETags.Add(new PartETag(result.PartNumber, result.ETag));

                    Logger.LogDebug("Multipart upload progress for file '{FileId}' with UploadId '{UploadId}': {CompletedParts}/{TotalParts} completed.",
                        args.FileId, uploadId, partETags.Count, partCount);
                }

                var completeMultipartUploadResponse = await Task<CompleteMultipartUploadResponse>.Factory.FromAsync(
                    (callback, state) => obsClient.BeginCompleteMultipartUpload(new CompleteMultipartUploadRequest()
                    {
                        BucketName = containerName,
                        ObjectKey = objectKey,
                        UploadId = uploadId,
                        PartETags = partETags
                    }, callback, state),
                    obsClient.EndCompleteMultipartUpload,
                    null);

                Logger.LogInformation("Multipart upload completed for file '{FileId}' with key '{ObjectKey}'. ETag: {ETag}",
                    args.FileId, completeMultipartUploadResponse.ObjectKey, completeMultipartUploadResponse.ETag);
                return args.FileId;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Multipart upload failed for file '{FileId}' with UploadId '{UploadId}'. Bucket: {BucketName}, Key: {ObjectKey}",
                    args.FileId, uploadId, containerName, objectKey);
                await AbortMultipartUploadSilentlyAsync(obsClient, containerName, objectKey, uploadId);
                throw;
            }
        }

        protected virtual async Task AbortMultipartUploadSilentlyAsync(
            ObsClient obsClient,
            string containerName,
            string objectKey,
            string uploadId)
        {
            try
            {
                await Task<AbortMultipartUploadResponse>.Factory.FromAsync(
                    (callback, state) => obsClient.BeginAbortMultipartUpload(new AbortMultipartUploadRequest()
                    {
                        BucketName = containerName,
                        ObjectKey = objectKey,
                        UploadId = uploadId
                    }, callback, state),
                    obsClient.EndAbortMultipartUpload,
                    null);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to abort multipart upload '{UploadId}'. Bucket: {BucketName}, Key: {ObjectKey}", uploadId, containerName, objectKey);
            }
        }

        protected class ObsClientLease
        {
            public ObsClient Client { get; }

            public ObsClientLease(ObsClient client)
            {
                Client = client;
            }

            public virtual void Return()
            {
            }
        }

    }
}

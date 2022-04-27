using OBS;
using OBS.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsFileProvider : FileProviderBase, ITransientDependency
    {
        protected IClock Clock { get; }
        protected IObsClientFactory ObsClientFactory { get; }
        protected IObsFileNameCalculator ObsFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public ObsFileProvider(
            IClock clock,
            IObsClientFactory obsClientFactory,
            IObsFileNameCalculator obsFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
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

        public override Task<string> SaveAsync(FileProviderSaveArgs args)
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

            var request = new PutObjectRequest()
            {
                BucketName = containerName,
                ObjectKey = objectKey,
                InputStream = args.FileStream
            };

            obsClient.PutObject(request);
            return Task.FromResult(args.FileId);
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

            if (!FileExistsAsync(obsClient, containerName, objectKey))
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

    }
}

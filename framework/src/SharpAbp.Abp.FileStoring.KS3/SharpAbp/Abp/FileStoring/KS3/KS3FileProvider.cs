using KS3;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using KS3SDK = KS3;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public class KS3FileProvider : FileProviderBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IClock Clock { get; }
        protected IKS3ClientFactory KS3ClientFactory { get; }
        protected IKS3FileNameCalculator KS3FileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public KS3FileProvider(
            ILogger<KS3FileProvider> logger,
            IClock clock,
            IKS3ClientFactory ks3ClientFactory,
            IKS3FileNameCalculator ks3FileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Logger = logger;
            Clock = clock;
            KS3ClientFactory = ks3ClientFactory;
            KS3FileNameCalculator = ks3FileNameCalculator;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override string Provider => KS3FileProviderConfigurationNames.ProviderName;

        protected virtual IKS3 GetKS3Client(FileContainerConfiguration fileContainerConfiguration)
        {
            var ks3Config = fileContainerConfiguration.GetKS3Configuration();
            return KS3ClientFactory.Create(ks3Config);
        }

        protected virtual IKS3 GetKS3Client(KS3FileProviderConfiguration ks3Config)
        {
            return KS3ClientFactory.Create(ks3Config);
        }

        public override Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = KS3FileNameCalculator.Calculate(args);
            var ks3Config = args.Configuration.GetKS3Configuration();
            var ks3Client = GetKS3Client(ks3Config);
            if (!args.OverrideExisting && FileExists(ks3Client, containerName, fileName))
            {
                throw new FileAlreadyExistsException($"Saving FILE '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }
            if (ks3Config.CreateContainerIfNotExists)
            {
                if (!BucketExist(ks3Client, containerName))
                {
                    ks3Client.CreateBucket(containerName);
                }
            }

            ks3Client.PutObject(containerName, fileName, args.FileStream, new KS3SDK.Model.ObjectMetadata());
            return Task.FromResult(args.FileId);
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = KS3FileNameCalculator.Calculate(args);
            var ks3Client = GetKS3Client(args.Configuration);
            if (!FileExists(ks3Client, containerName, fileName))
            {
                return Task.FromResult(false);
            }
            ks3Client.DeleteObject(containerName, fileName);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = KS3FileNameCalculator.Calculate(args);
            var ks3Client = GetKS3Client(args.Configuration);
            return Task.FromResult(FileExists(ks3Client, containerName, fileName));
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var blobName = KS3FileNameCalculator.Calculate(args);
            var ks3Client = GetKS3Client(args.Configuration);
            if (!FileExists(ks3Client, containerName, blobName))
            {
                return null;
            }
            var result = ks3Client.GetObject(containerName, blobName);

            return await TryCopyToMemoryStreamAsync(result.ObjectContent, args.CancellationToken);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = KS3FileNameCalculator.Calculate(args);
            var ks3Client = GetKS3Client(args.Configuration);
            if (!FileExists(ks3Client, containerName, fileName))
            {
                return false;
            }

            var result = ks3Client.GetObject(containerName, fileName);
            await TryWriteToFileAsync(result.ObjectContent, args.Path, args.CancellationToken);
            return true;
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return Task.FromResult(string.Empty);
            }

            var containerName = GetContainerName(args);
            var fileName = KS3FileNameCalculator.Calculate(args);
            var ks3Client = GetKS3Client(args.Configuration);

            if (!FileExists(ks3Client, containerName, fileName))
            {
                return Task.FromResult(string.Empty);
            }

            var datetime = args.Expires ?? Clock.Now.AddSeconds(600);
            var uri = ks3Client.GeneratePresignedUrl(containerName, fileName, datetime);

            return Task.FromResult(uri.ToString());
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

    }
}

using Aliyun.OSS;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public class AliyunFileProvider : FileProviderBase, ITransientDependency
    {
        protected IClock Clock { get; }
        protected IOssClientFactory OssClientFactory { get; }
        protected IAliyunFileNameCalculator AliyunFileNameCalculator { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }

        public AliyunFileProvider(
            IClock clock,
            IOssClientFactory ossClientFactory,
            IAliyunFileNameCalculator aliyunFileNameCalculator,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
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

        public override Task<string> SaveAsync(FileProviderSaveArgs args)
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

            ossClient.PutObject(containerName, objectKey, args.FileStream);
            return Task.FromResult(args.FileId);
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

    }
}

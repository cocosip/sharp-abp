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
            var aliyunConfig = fileContainerConfiguration.GetAliyunConfiguration();
            return OssClientFactory.Create(aliyunConfig);
        }

        protected virtual IOss GetOssClient(AliyunFileProviderConfiguration aliyunConfig)
        {
            return OssClientFactory.Create(aliyunConfig);
        }


        public override Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = AliyunFileNameCalculator.Calculate(args);
            var aliyunConfig = args.Configuration.GetAliyunConfiguration();
            var ossClient = GetOssClient(aliyunConfig);
            if (!args.OverrideExisting && FileExistsAsync(ossClient, containerName, fileName))
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

            ossClient.PutObject(containerName, fileName, args.FileStream);
            return Task.FromResult(fileName);
        }

        public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            if (!FileExistsAsync(ossClient, containerName, fileName))
            {
                return Task.FromResult(false);
            }
            ossClient.DeleteObject(containerName, fileName);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var containerName = GetContainerName(args);
            var blobName = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            return Task.FromResult(FileExistsAsync(ossClient, containerName, blobName));
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var containerName = GetContainerName(args);
            var blobName = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            if (!FileExistsAsync(ossClient, containerName, blobName))
            {
                return null;
            }
            var result = ossClient.GetObject(containerName, blobName);
            var memoryStream = new MemoryStream();
            await result.Content.CopyToAsync(memoryStream);
            return memoryStream;
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            if (!FileExistsAsync(ossClient, containerName, fileName))
            {
                return false;
            }

            var result = ossClient.GetObject(containerName, fileName);

            using (var fs = new FileStream(args.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await result.Content.CopyToAsync(fs);
            }
            return true;
        }

        public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            var containerName = GetContainerName(args);
            var fileName = AliyunFileNameCalculator.Calculate(args);
            var ossClient = GetOssClient(args.Configuration);
            if (!FileExistsAsync(ossClient, containerName, fileName))
            {
                return Task.FromResult(string.Empty);
            }

            var datetime = args.Expires ?? Clock.Now.AddSeconds(600);
            var uri = ossClient.GeneratePresignedUri(containerName, fileName, datetime);

            return Task.FromResult(uri.ToString());
        }


        private string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAliyunConfiguration();
            return configuration.BucketName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.BucketName);
        }

        private bool FileExistsAsync(IOss ossClient, string containerName, string fileName)
        {
            // Make sure Blob Container exists.
            return ossClient.DoesBucketExist(containerName) &&
                   ossClient.DoesObjectExist(containerName, fileName);
        }

    }
}

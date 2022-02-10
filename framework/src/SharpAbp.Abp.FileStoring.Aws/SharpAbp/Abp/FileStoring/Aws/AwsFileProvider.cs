using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public class AwsFileProvider : FileProviderBase, ITransientDependency
    {
        public override string Provider => AwsFileProviderConfigurationNames.ProviderName;

        protected IClock Clock { get; }
        protected IAwsFileNameCalculator AwsFileNameCalculator { get; }
        protected IAmazonS3ClientFactory AmazonS3ClientFactory { get; }
        protected IFileNormalizeNamingService FileNormalizeNamingService { get; }
        public AwsFileProvider(
            IClock clock,
            IAwsFileNameCalculator awsFileNameCalculator,
            IAmazonS3ClientFactory amazonS3ClientFactory,
            IFileNormalizeNamingService fileNormalizeNamingService)
        {
            Clock = clock;
            AwsFileNameCalculator = awsFileNameCalculator;
            AmazonS3ClientFactory = amazonS3ClientFactory;
            FileNormalizeNamingService = fileNormalizeNamingService;
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var fileName = AwsFileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetAwsConfiguration();
            var containerName = GetContainerName(args);

            using var amazonS3Client = await GetAmazonS3Client(args);
            if (!args.OverrideExisting && await FileExistsAsync(amazonS3Client, containerName, fileName))
            {
                throw new FileAlreadyExistsException(
                    $"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateContainerIfNotExists)
            {
                await CreateContainerIfNotExists(amazonS3Client, containerName);
            }

            await amazonS3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = containerName,
                Key = fileName,
                InputStream = args.FileStream
            });

            return args.FileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var fileName = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            using var amazonS3Client = await GetAmazonS3Client(args);
            if (!await FileExistsAsync(amazonS3Client, containerName, fileName))
            {
                return false;
            }

            await amazonS3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = containerName,
                Key = fileName
            });

            return true;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var fileName = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            using var amazonS3Client = await GetAmazonS3Client(args);
            return await FileExistsAsync(amazonS3Client, containerName, fileName);
        }
        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var fileName = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            using var amazonS3Client = await GetAmazonS3Client(args);
            if (!await FileExistsAsync(amazonS3Client, containerName, fileName))
            {
                return null;
            }

            var response = await amazonS3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = containerName,
                Key = fileName
            });

            return await TryCopyToMemoryStreamAsync(response.ResponseStream, args.CancellationToken);
        }

        public override async Task<bool> DownloadAsync(FileProviderDownloadArgs args)
        {
            var fileName = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            using var amazonS3Client = await GetAmazonS3Client(args);
            if (!await FileExistsAsync(amazonS3Client, containerName, fileName))
            {
                return false;
            }

            var response = await amazonS3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = containerName,
                Key = fileName
            });

            await TryWriteToFileAsync(response.ResponseStream, args.Path, args.CancellationToken);
            return true;
        }

        public override async Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
        {
            if (!args.Configuration.HttpAccess)
            {
                return string.Empty;
            }

            var fileName = AwsFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);

            using var amazonS3Client = await GetAmazonS3Client(args);
            if (!await FileExistsAsync(amazonS3Client, containerName, fileName))
            {
                return string.Empty;
            }

            var datetime = args.Expires ?? Clock.Now.AddSeconds(600);
            var url = amazonS3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
            {
                BucketName = containerName,
                Key = fileName,
                Expires = datetime
            });

            return url;
        }


        protected virtual async Task<bool> FileExistsAsync(AmazonS3Client amazonS3Client, string containerName, string fileName)
        {
            // Make sure file Container exists.
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, containerName))
            {
                return false;
            }

            try
            {
                await amazonS3Client.GetObjectMetadataAsync(containerName, fileName);
            }
            catch (Exception ex)
            {
                if (ex is AmazonS3Exception)
                {
                    return false;
                }

                throw;
            }
            return true;
        }

        protected virtual async Task CreateContainerIfNotExists(AmazonS3Client amazonS3Client, string containerName)
        {
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, containerName))
            {
                await amazonS3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = containerName
                });
            }
        }

        protected virtual async Task<AmazonS3Client> GetAmazonS3Client(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAwsConfiguration();
            return await AmazonS3ClientFactory.GetAmazonS3Client(configuration);
        }

        protected virtual string GetContainerName(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetAwsConfiguration();
            return configuration.ContainerName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : FileNormalizeNamingService.NormalizeContainerName(args.Configuration, configuration.ContainerName);
        }


    }
}

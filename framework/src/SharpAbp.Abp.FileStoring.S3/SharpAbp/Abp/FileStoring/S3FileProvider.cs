using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Multiplex;
using AmazonKS3;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class S3FileProvider : FileProviderBase, ITransientDependency
    {
        protected IS3FileNameCalculator S3FileNameCalculator { get; }

        protected IS3ClientFactory S3ClientFactory { get; }

        public S3FileProvider(IS3FileNameCalculator s3FileNameCalculator, IS3ClientFactory s3ClientFactory)
        {
            S3FileNameCalculator = s3FileNameCalculator;
            S3ClientFactory = s3ClientFactory;
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var fileId = S3FileNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetS3Configuration();
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            if (!args.OverrideExisting && await FileExistsAsync(client, containerName, fileId))
            {
                throw new FileAlreadyExistsException($"Saving File '{args.FileId}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateBucketIfNotExists)
            {
                await CreateBucketIfNotExists(client, containerName);
            }

            await client.PutObjectAsync(new PutObjectRequest()
            {
                BucketName = containerName,
                Key = fileId,
                InputStream = args.FileStream,
                AutoCloseStream = true
            });

            return fileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            if (await FileExistsAsync(client, containerName, fileName))
            {
                await client.DeleteAsync(containerName, fileName, null);
                return true;
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            return await FileExistsAsync(client, containerName, fileName);
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            var fileName = S3FileNameCalculator.Calculate(args);
            var client = GetS3Client(args);
            var containerName = GetContainerName(args);

            if (!await FileExistsAsync(client, containerName, fileName))
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            var getObjectResponse = await client.GetObjectAsync(containerName, fileName);
            if (getObjectResponse.ResponseStream != null)
            {
                await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
            }

            return memoryStream;
        }

        protected virtual IAmazonS3 GetS3Client(FileProviderArgs args)
        {
            var configuration = args.Configuration.GetS3Configuration();

            var amazonS3 = S3ClientFactory.GetOrAddClient(configuration.AccessKeyId, configuration.SecretAccessKey, () =>
             {
                 var s3ClientDescriptor = new S3ClientDescriptor()
                 {
                     VendorType = (S3VendorType)configuration.VendorType,
                     AccessKeyId = configuration.AccessKeyId,
                     SecretAccessKey = configuration.SecretAccessKey,
                     ClientCount = configuration.ClientCount,
                 };

                 if (configuration.VendorType == (int)S3VendorType.KS3)
                 {
                     s3ClientDescriptor.Config = new AmazonS3Config()
                     {
                         ServiceURL = configuration.ServerUrl,
                         ForcePathStyle = configuration.ForcePathStyle,
                         SignatureVersion = configuration.SignatureVersion
                     };
                 }
                 else
                 {
                     s3ClientDescriptor.Config = new AmazonKS3Config()
                     {
                         ServiceURL = configuration.ServerUrl,
                         ForcePathStyle = configuration.ForcePathStyle,
                         SignatureVersion = configuration.SignatureVersion
                     };
                 }

                 return s3ClientDescriptor;
             });

            return amazonS3;
        }

        protected virtual async Task CreateBucketIfNotExists(IAmazonS3 client, string containerName)
        {
            await client.EnsureBucketExistsAsync(containerName);
        }

        private async Task<bool> FileExistsAsync(IAmazonS3 client, string containerName, string fileName)
        {
            // Make sure Blob Container exists.
            if (await client.DoesS3BucketExistAsync(containerName))
            {
                try
                {
                    var getObjectMetadataResponse = await client.GetObjectMetadataAsync(containerName, fileName);

                }
                catch (Exception e)
                {
                    if (e is Amazon.S3.AmazonS3Exception)
                    {
                        return false;
                    }

                    throw;
                }

                return true;
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

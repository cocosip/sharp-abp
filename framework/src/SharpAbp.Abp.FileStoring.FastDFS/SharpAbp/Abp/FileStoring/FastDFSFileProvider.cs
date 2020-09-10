using FastDFSCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class FastDFSFileProvider : FileProviderBase, ITransientDependency
    {
        protected IFastDFSFileNameCalculator FastDFSFileNameCalculator { get; }
        protected IFastDFSClient FastDFSClient { get; }

        public FastDFSFileProvider(IFastDFSFileNameCalculator fastDFSFileNameCalculator, IFastDFSClient fastDFSClient)
        {
            FastDFSFileNameCalculator = fastDFSFileNameCalculator;
            FastDFSClient = fastDFSClient;
        }

        public override async Task<string> SaveAsync(FileProviderSaveArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            var containerName = GetContainerName(configuration, args);
            var storageNode = await FastDFSClient.GetStorageNodeAsync(containerName, configuration.ClusterName);
            var fileId = await FastDFSClient.UploadFileAsync(storageNode, args.FileStream, "", configuration.ClusterName);
            return fileId;
        }

        public override async Task<bool> DeleteAsync(FileProviderDeleteArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            var fileName = FastDFSFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);
            return await FastDFSClient.RemoveFileAsync(containerName, fileName, configuration.ClusterName);
        }

        public override async Task<bool> ExistsAsync(FileProviderExistsArgs args)
        {
            var configuration = args.Configuration.GetFastDFSConfiguration();
            var fileName = FastDFSFileNameCalculator.Calculate(args);
            var containerName = GetContainerName(configuration, args);

            var storageNode = await FastDFSClient.GetStorageNodeAsync(containerName, configuration.ClusterName);
            var fileInfo = await FastDFSClient.GetFileInfo(storageNode, fileName, configuration.ClusterName);
            return fileInfo != null;
        }

        public override async Task<Stream> GetOrNullAsync(FileProviderGetArgs args)
        {
            //var fileName = FastDFSFileNameCalculator.Calculate(args);
            //var client = GetS3Client(args);
            //var containerName = GetContainerName(args);

            //if (!await FileExistsAsync(client, containerName, fileName))
            //{
            //    return null;
            //}

            //var memoryStream = new MemoryStream();
            //var getObjectResponse = await client.GetObjectAsync(containerName, fileName);
            //if (getObjectResponse.ResponseStream != null)
            //{
            //    await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
            //}
            var memoryStream = new MemoryStream();
            return await Task.FromResult(memoryStream);
        }


        private static string GetContainerName(FastDFSFileProviderConfiguration configuration, FileProviderArgs args)
        {
            return configuration.GroupName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : configuration.GroupName;
        }
    }
}

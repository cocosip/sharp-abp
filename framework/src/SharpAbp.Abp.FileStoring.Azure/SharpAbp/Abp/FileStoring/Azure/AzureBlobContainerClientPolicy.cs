using Azure.Storage.Blobs;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.Azure
{
    public class AzureBlobContainerClientPolicy : IObjectPoolPolicy<BlobContainerClient>
    {
        protected string ConnectionString { get; }
        protected string ContainerName { get; }

        public AzureBlobContainerClientPolicy(
            string connectionString,
            string containerName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;
        }

        public BlobContainerClient Create()
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            return blobServiceClient.GetBlobContainerClient(ContainerName);
        }

        public bool Return(BlobContainerClient obj)
        {
            return obj != null;
        }
    }
}

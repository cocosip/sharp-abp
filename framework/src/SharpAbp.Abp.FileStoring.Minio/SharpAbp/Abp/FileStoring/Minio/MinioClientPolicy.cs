using Minio;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class MinioClientPolicy : IObjectPoolPolicy<IMinioClient>
    {
        protected MinioFileProviderConfiguration MinioFileProviderConfiguration { get; }

        public MinioClientPolicy(
            MinioFileProviderConfiguration minioFileProviderConfiguration)
        {
            MinioFileProviderConfiguration = minioFileProviderConfiguration;
        }

        public IMinioClient Create()
        {
            var minioConfiguration = MinioFileProviderConfiguration;
            var client = new MinioClient()
                .WithEndpoint(minioConfiguration.EndPoint)
                .WithCredentials(minioConfiguration.AccessKey, minioConfiguration.SecretKey);
            if (minioConfiguration.WithSSL)
            {
                client.WithSSL();
            }
            client.Build();
            return client;
        }

        public bool Return(IMinioClient obj)
        {
            return obj != null;
        }
    }
}

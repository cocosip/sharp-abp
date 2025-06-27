using System;
using Microsoft.Extensions.ObjectPool;
using Minio;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class MinioClientPolicy : IPooledObjectPolicy<IMinioClient>
    {
        protected IServiceProvider ServiceProvider { get; }
        protected MinioFileProviderConfiguration MinioFileProviderConfiguration { get; }

        public MinioClientPolicy(
            IServiceProvider serviceProvider,
            MinioFileProviderConfiguration minioFileProviderConfiguration)
        {
            ServiceProvider = serviceProvider;
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

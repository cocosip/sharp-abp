using System;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3ClientPolicy : IPooledObjectPolicy<IAmazonS3>
    {
        protected IServiceProvider ServiceProvider { get; }
        protected S3FileProviderConfiguration S3FileProviderConfiguration { get; }
        public S3ClientPolicy(
            IServiceProvider serviceProvider,
            S3FileProviderConfiguration s3FileProviderConfiguration)
        {
            ServiceProvider = serviceProvider;
            S3FileProviderConfiguration = s3FileProviderConfiguration;
        }

        public IAmazonS3 Create()
        {
            using var scope = ServiceProvider.CreateScope();
            var s3ClientFactory = scope.ServiceProvider.GetRequiredService<IS3ClientFactory>();
            return s3ClientFactory.Create(S3FileProviderConfiguration);
        }

        public bool Return(IAmazonS3 obj)
        {
            return obj != null;
        }
    }
}

using System;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public class AmazonS3ClientPolicy : IPooledObjectPolicy<IAmazonS3>
    {
        protected IServiceProvider ServiceProvider { get; }
        protected AwsFileProviderConfiguration AwsFileProviderConfiguration { get; }
        public AmazonS3ClientPolicy(
            IServiceProvider serviceProvider,
            AwsFileProviderConfiguration awsFileProviderConfiguration)
        {
            ServiceProvider = serviceProvider;
            AwsFileProviderConfiguration = awsFileProviderConfiguration;
        }

        public IAmazonS3 Create()
        {
            using var scope = ServiceProvider.CreateScope();
            var amazonS3ClientFactory = scope.ServiceProvider.GetRequiredService<IAmazonS3ClientFactory>();
            return AsyncHelper.RunSync(() =>
            {
                return amazonS3ClientFactory.GetAmazonS3Client(AwsFileProviderConfiguration);
            });
        }

        public bool Return(IAmazonS3 obj)
        {
            return obj != null;
        }
    }
}

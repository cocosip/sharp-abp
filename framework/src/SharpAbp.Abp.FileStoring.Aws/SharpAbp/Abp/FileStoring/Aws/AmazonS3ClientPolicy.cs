using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.ObjectPool;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public class AmazonS3ClientPolicy : IAsyncObjectPoolPolicy<IAmazonS3>
    {
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected AwsFileProviderConfiguration AwsFileProviderConfiguration { get; }

        public AmazonS3ClientPolicy(
            IServiceScopeFactory serviceScopeFactory,
            AwsFileProviderConfiguration awsFileProviderConfiguration)
        {
            ServiceScopeFactory = serviceScopeFactory;
            AwsFileProviderConfiguration = awsFileProviderConfiguration;
        }

        public async ValueTask<IAmazonS3> CreateAsync()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var amazonS3ClientFactory = scope.ServiceProvider.GetRequiredService<IAmazonS3ClientFactory>();
            return await amazonS3ClientFactory.GetAmazonS3Client(AwsFileProviderConfiguration);
        }

        public bool Return(IAmazonS3 obj)
        {
            return obj != null;
        }
    }
}

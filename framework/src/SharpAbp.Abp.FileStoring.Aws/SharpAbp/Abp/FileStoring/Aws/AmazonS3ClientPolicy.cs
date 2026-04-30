using Amazon.S3;
using SharpAbp.Abp.ObjectPool;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public class AmazonS3ClientPolicy : IObjectPoolPolicy<IAmazonS3>
    {
        protected IAmazonS3ClientFactory AmazonS3ClientFactory { get; }
        protected AwsFileProviderConfiguration AwsFileProviderConfiguration { get; }

        public AmazonS3ClientPolicy(
            IAmazonS3ClientFactory amazonS3ClientFactory,
            AwsFileProviderConfiguration awsFileProviderConfiguration)
        {
            AmazonS3ClientFactory = amazonS3ClientFactory;
            AwsFileProviderConfiguration = awsFileProviderConfiguration;
        }

        public IAmazonS3 Create()
        {
            return AsyncHelper.RunSync(() =>
            {
                return AmazonS3ClientFactory.GetAmazonS3Client(AwsFileProviderConfiguration);
            });
        }

        public bool Return(IAmazonS3 obj)
        {
            return obj != null;
        }
    }
}

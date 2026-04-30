using Amazon.S3;
using SharpAbp.Abp.ObjectPool;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3ClientPolicy : IObjectPoolPolicy<IAmazonS3>
    {
        protected IS3ClientFactory S3ClientFactory { get; }
        protected S3FileProviderConfiguration S3FileProviderConfiguration { get; }

        public S3ClientPolicy(
            IS3ClientFactory s3ClientFactory,
            S3FileProviderConfiguration s3FileProviderConfiguration)
        {
            S3ClientFactory = s3ClientFactory;
            S3FileProviderConfiguration = s3FileProviderConfiguration;
        }

        public IAmazonS3 Create()
        {
            return S3ClientFactory.Create(S3FileProviderConfiguration);
        }

        public bool Return(IAmazonS3 obj)
        {
            return obj != null;
        }
    }
}

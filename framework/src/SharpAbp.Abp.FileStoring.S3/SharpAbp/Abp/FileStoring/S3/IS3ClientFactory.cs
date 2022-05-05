using Amazon.S3;

namespace SharpAbp.Abp.FileStoring.S3
{
    public interface IS3ClientFactory
    {
        IAmazonS3 Create(S3FileProviderConfiguration configuration);
    }
}

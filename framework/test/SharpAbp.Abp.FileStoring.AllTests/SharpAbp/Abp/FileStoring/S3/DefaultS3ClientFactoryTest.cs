using System;
using Amazon.S3.Model;
using Xunit;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class DefaultS3ClientFactoryTest
    {
        [Fact]
        public void Create_Should_Use_Configured_AuthenticationRegion_For_SigV4()
        {
            var containerConfiguration = new FileContainerConfiguration();
            var configuration = containerConfiguration.GetS3Configuration();
            configuration.ServerUrl = "http://127.0.0.1:9000";
            configuration.AccessKeyId = "AccessKeyId";
            configuration.SecretAccessKey = "SecretAccessKey";
            configuration.AuthenticationRegion = "cn-north-1";
            configuration.ForcePathStyle = true;

            using var client = new DefaultS3ClientFactory().Create(configuration);
            var url = client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = "bucket",
                Key = "object",
                Expires = DateTime.UtcNow.AddMinutes(5)
            });
            var decodedUrl = Uri.UnescapeDataString(url);

            Assert.Contains("X-Amz-Algorithm=AWS4-HMAC-SHA256", decodedUrl);
            Assert.Contains("/cn-north-1/s3/aws4_request", decodedUrl);
        }
    }
}

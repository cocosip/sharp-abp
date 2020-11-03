using AmazonKS3;
using AutoS3;
using Xunit;

namespace SharpAbp.Abp.AutoS3.KS3
{
    public class ClientFactoryTest : AbpAutoS3KS3TestBase
    {
        private readonly IS3ClientFactory _s3ClientFactory;

        public ClientFactoryTest()
        {
            _s3ClientFactory = GetRequiredService<IS3ClientFactory>();
        }


        [Fact]
        public void Get_Test()
        {

            var client = _s3ClientFactory.GetOrAddClient("123456", "123456", () => new S3ClientConfiguration()
            {
                AccessKeyId = "111111",
                SecretAccessKey = "222222",
                Vendor = S3VendorType.Amazon,
                MaxClient = 2,
                Config = new AmazonKS3Config()
                {
                    ServiceURL = "http://ks3-cn-beijing.ksyun.com",
                    SignatureVersion = "2.0"
                }
            });

            Assert.NotNull(client);

        }
    }
}

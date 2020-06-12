using Amazon.S3.Multiplex;
using SharpAbp.AmazonKS3;
using Volo.Abp.Modularity;

namespace SharpAbp.Amazon.S3.Multiplex
{
    [DependsOn(typeof(SharpAbpAmazonKS3Module),
        typeof(SharpAbpAmazonS3MultiplexModule))]
    public class SharpAbpAmazonS3MultiplexKS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddS3MultiplexKS3Builder();
        }

    }
}

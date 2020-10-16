using Amazon.S3.Multiplex;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AmazonS3.KS3
{
    [DependsOn(
        typeof(AbpAmazonS3Module)
    )]
    public class AbpAmazonS3KS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddS3MultiplexKS3Builder();
        }
    }
}

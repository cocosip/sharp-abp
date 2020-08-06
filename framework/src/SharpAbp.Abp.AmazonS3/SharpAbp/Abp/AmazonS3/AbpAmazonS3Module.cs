using Amazon.S3.Multiplex;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AmazonS3
{
    public class AbpAmazonS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddS3Multiplex();
        }
    }
}

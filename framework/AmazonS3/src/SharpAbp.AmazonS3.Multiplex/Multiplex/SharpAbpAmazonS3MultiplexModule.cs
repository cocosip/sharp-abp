using Amazon.S3.Multiplex;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Amazon.S3.Multiplex
{
    public class SharpAbpAmazonS3MultiplexModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddS3Multiplex();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureS3Multiplex();
        }
    }
}

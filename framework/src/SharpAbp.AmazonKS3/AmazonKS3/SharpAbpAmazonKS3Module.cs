using AmazonKS3;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.AmazonKS3
{

    public class SharpAbpAmazonKS3Module : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAmazonKS3();
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            context.ServiceProvider.ConfigureAmazonKS3();
        }
    }
}

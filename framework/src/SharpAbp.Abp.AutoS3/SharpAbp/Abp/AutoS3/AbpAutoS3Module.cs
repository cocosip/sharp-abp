using AutoS3;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AutoS3
{
    public class AbpAutoS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoS3();
        }
    }
}

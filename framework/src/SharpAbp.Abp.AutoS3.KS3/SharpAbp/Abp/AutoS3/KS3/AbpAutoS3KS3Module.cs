using AutoS3.KS3;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AutoS3.KS3
{
    [DependsOn(
        typeof(AbpAutoS3Module)
        )]
    public class AbpAutoS3KS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoKS3();
        }
    }
}

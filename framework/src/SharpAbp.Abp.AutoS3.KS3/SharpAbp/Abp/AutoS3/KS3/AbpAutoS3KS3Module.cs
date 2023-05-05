using AutoS3.KS3;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.AutoS3.KS3
{
    [DependsOn(
        typeof(AbpAutoS3Module)
        )]
    public class AbpAutoS3KS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAutoKS3();
            return Task.CompletedTask;
        }
    }
}

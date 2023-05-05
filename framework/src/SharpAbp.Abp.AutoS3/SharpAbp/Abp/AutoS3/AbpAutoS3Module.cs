using AutoS3;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.AutoS3
{
    public class AbpAutoS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }


        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAutoS3();
            return Task.CompletedTask;
        }
    }
}

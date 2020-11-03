using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.CAP
{
    public class AbpCapModule : AbpModule
    {
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddCap(options => { });
        }
    
    }
}

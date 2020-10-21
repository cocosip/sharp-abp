using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    [DependsOn(
         typeof(AbpMicroDiscoveryConsulModule),
         typeof(AbpTestBaseModule),
         typeof(AbpAutofacModule)
         )]
    public class AbpMicroDiscoveryConsulTestModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpMicroDiscoveryConsulOptions>(configuration.GetSection("ConsulDiscovery"));

            //context.Services.Replace()

        }
    }
}

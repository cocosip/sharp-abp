using SharpAbp.Abp.Micro.ServiceDiscovery;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    [DependsOn(
       typeof(AbpMicroDiscoveryModule)
    )]
    public class AbpMicroDiscoveryConsulModule : AbpModule
    {


    }
}

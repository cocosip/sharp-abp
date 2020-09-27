using SharpAbp.Abp.Consul;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    [DependsOn(
        typeof(AbpMicroDiscoveryModule),
        typeof(AbpConsulModule)
    )]
    public class AbpMicroDiscoveryConsulModule : AbpModule
    {

    }
}

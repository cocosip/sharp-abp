using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    [DependsOn(
        typeof(AbpMicroModule)
    )]
    public class AbpMicroLoadBalancerModule : AbpModule
    {

    }
}

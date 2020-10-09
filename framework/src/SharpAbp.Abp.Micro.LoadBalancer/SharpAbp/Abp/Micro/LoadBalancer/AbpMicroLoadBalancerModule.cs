using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.LoadBalance
{
    [DependsOn(
        typeof(AbpMicroModule)
    )]
    public class AbpMicroLoadBalancerModule : AbpModule
    {

    }
}

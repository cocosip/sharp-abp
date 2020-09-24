using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Registry
{
    [DependsOn(
        typeof(AbpMicroModule)
    )]
    public class AbpMicroRegistryModule : AbpModule
    {

    }
}

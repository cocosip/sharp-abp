using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Basic test module for MassTransit without provider-specific modules to avoid initialization issues
    /// </summary>
    [DependsOn(
        typeof(AbpMassTransitModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
      )]
    public class AbpMassTransitBasicTestModule: AbpModule
    {

    }
}
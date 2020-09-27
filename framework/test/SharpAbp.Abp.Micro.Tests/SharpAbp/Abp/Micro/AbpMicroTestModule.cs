using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro
{
    [DependsOn(
        typeof(AbpMicroModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class AbpMicroTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {


        }
    }
}

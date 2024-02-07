using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Crypto
{
    [DependsOn(
        typeof(AbpCryptoModule),
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule)
       )]
    public class AbpCryptoTestModule : AbpModule
    {
    }
}

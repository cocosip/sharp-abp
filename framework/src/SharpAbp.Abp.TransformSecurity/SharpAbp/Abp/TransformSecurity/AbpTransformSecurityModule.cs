using SharpAbp.Abp.Crypto;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace SharpAbp.Abp.TransformSecurity
{
    [DependsOn(
        typeof(AbpCryptoModule),
        typeof(AbpObjectExtendingModule)
        )]
    public class AbpTransformSecurityModule : AbpModule
    {

    }
}

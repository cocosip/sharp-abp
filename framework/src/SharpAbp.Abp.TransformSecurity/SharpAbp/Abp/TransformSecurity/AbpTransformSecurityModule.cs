using SharpAbp.Abp.TransformSecurity.Abstractions;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.TransformSecurity
{
    [DependsOn(
        typeof(AbpTransformSecurityAbstractionsModule)
        )]
    public class AbpTransformSecurityModule : AbpModule
    {

    }
}

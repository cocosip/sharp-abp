using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Validation
{
    [DependsOn(
        typeof(AbpValidationModule)
        )]
    public class SharpAbpValidationModule : AbpModule
    {

    }
}

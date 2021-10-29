using Volo.Abp.Modularity;

namespace SharpAbp.Abp.IdentityModel
{
    [DependsOn(
        typeof(Volo.Abp.IdentityModel.AbpIdentityModelModule)
        )]
    public class AbpIdentityModelModule : AbpModule
    {

    }
}

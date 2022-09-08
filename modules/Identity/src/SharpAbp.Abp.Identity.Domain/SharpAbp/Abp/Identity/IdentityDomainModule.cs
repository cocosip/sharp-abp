using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(AbpIdentityDomainModule),
        typeof(IdentityDomainSharedModule),
        typeof(AbpSettingManagementDomainModule)
        )]
    public class IdentityDomainModule : AbpModule
    {

    }
}

using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
        typeof(FileStoringManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class FileStoringManagementApplicationContractsModule : AbpModule
    {

    }
}

using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
        typeof(FileStoringManagementApplicationContractsModule),
        typeof(FileStoringManagementDomainModule),
        typeof(AbpDddApplicationModule)
        )]
    public class FileStoringManagementApplicationModule : AbpModule
    {
    }
}

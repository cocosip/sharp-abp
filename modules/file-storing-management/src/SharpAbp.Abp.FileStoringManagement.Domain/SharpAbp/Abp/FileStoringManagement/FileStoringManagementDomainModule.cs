using SharpAbp.Abp.FileStoring;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpFileStoringModule),
        typeof(FileStoringManagementDomainSharedModule)
        )]
    public class FileStoringManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }

    }
}

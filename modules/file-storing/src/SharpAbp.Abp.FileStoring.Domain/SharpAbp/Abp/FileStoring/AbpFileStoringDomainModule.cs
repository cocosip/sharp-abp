using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(typeof(AbpFileStoringDomainSharedModule))]
    public class AbpFileStoringDomainModule : AbpModule
    {

    }
}

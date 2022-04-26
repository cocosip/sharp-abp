using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring.Obs
{
    [DependsOn(
        typeof(AbpFileStoringModule)
        )]
    public class AbpFileStoringObsModule : AbpModule
    {
    }
}

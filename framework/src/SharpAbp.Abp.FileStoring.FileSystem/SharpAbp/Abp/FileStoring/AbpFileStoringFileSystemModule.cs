using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
       typeof(AbpFileStoringModule)
       )]
    public class AbpFileStoringFileSystemModule : AbpModule
    {
    }
}

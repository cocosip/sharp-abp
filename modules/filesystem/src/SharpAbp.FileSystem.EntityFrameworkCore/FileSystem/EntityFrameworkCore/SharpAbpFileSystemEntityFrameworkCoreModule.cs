using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.FileSystem.EntityFrameworkCore
{

    [DependsOn(
       typeof(SharpAbpFileSystemModule),
       typeof(AbpEntityFrameworkCoreModule)
    )]
    public class SharpAbpFileSystemEntityFrameworkCoreModule : AbpModule
    {
    }
}

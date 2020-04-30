using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace SharpAbp.FileSystem
{
    [DependsOn(
      //typeof(SharpAbpFileSystemModule),
      typeof(AbpDddDomainModule)
    )]
    public class SharpAbpFileSystemDomainModule
    {
    }
}

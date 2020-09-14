using SharpAbp.Abp.FastDFS.DotNetty;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
      typeof(AbpFileStoringModule),
      typeof(AbpFastDFSDotNettyModule)
      )]
    public class AbpFileStoringFastDFSModule : AbpModule
    {

    }
}

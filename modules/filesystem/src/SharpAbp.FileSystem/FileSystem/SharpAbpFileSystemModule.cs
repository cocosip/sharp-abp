using SharpAbp.AmazonKS3;
using SharpAbp.DotCommon;
using SharpAbp.FastDFSCore;
using Volo.Abp.Modularity;

namespace SharpAbp.FileSystem
{
    /// <summary>Abp文件存储系统
    /// </summary>
    [DependsOn(typeof(SharpAbpDotCommonModule),
        typeof(SharpAbpAmazonKS3Module),
        typeof(SharpAbpFastDFSCoreModule)
        )]
    public class SharpAbpFileSystemModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFileSystem();
        }



    }
}

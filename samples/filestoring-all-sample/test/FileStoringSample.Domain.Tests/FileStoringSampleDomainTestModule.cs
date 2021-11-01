using FileStoringSample.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace FileStoringSample
{
    [DependsOn(
        typeof(FileStoringSampleEntityFrameworkCoreTestModule)
        )]
    public class FileStoringSampleDomainTestModule : AbpModule
    {

    }
}
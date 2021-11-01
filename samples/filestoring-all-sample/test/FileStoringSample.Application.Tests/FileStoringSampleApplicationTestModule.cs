using Volo.Abp.Modularity;

namespace FileStoringSample
{
    [DependsOn(
        typeof(FileStoringSampleApplicationModule),
        typeof(FileStoringSampleDomainTestModule)
        )]
    public class FileStoringSampleApplicationTestModule : AbpModule
    {

    }
}
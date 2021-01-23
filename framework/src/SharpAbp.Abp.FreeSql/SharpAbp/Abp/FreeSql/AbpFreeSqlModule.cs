using Volo.Abp.Domain;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeSql
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class AbpFreeSqlModule : AbpModule
    {

    }
}

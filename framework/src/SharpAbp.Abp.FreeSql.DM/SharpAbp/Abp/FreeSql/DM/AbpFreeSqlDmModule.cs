using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeSql.DM
{
    [DependsOn(
        typeof(AbpFreeSqlModule)
        )]
    public class AbpFreeSqlDmModule : AbpModule
    {
    }
}

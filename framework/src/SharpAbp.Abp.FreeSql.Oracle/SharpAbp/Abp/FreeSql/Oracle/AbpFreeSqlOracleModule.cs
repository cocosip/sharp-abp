using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeSql.Oracle
{
    [DependsOn(
        typeof(AbpFreeSqlModule)
        )]
    public class AbpFreeSqlOracleModule : AbpModule
    {
    }
}

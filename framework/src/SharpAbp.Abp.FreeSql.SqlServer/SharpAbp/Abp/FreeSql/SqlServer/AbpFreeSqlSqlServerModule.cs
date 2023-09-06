using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeSql.SqlServer
{
    [DependsOn(
        typeof(AbpFreeSqlModule)
        )]
    public class AbpFreeSqlSqlServerModule : AbpModule
    {
    }
}

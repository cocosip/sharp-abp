using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeSql.Sqlite
{
    [DependsOn(
        typeof(AbpFreeSqlModule)
        )]
    public class AbpFreeSqlSqliteModule : AbpModule
    {

    }
}

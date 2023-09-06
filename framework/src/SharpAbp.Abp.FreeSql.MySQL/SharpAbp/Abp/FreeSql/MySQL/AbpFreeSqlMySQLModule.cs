using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeSql.MySQL
{
    [DependsOn(
        typeof(AbpFreeSqlModule)
        )]
    public class AbpFreeSqlMySQLModule : AbpModule
    {
    }
}

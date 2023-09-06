using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeSql.PostgreSql
{
    [DependsOn(
        typeof(AbpFreeSqlModule)
        )]
    public class AbpFreeSqlPostgreSqlModule : AbpModule
    {
    }
}

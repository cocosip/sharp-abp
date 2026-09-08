using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Dapper.Sqlite
{
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperSqliteModule : AbpModule
    {
    }
}

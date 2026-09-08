using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Dapper.SqlServer
{
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperSqlServerModule : AbpModule
    {
    }
}

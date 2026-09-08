using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Dapper.MySQL
{
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperMySQLModule : AbpModule
    {
    }
}

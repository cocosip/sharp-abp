using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Dapper.DM
{
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperDmModule : AbpModule
    {
    }
}

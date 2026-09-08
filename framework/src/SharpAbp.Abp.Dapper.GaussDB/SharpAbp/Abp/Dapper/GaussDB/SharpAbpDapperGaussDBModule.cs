using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Dapper.GaussDB
{
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperGaussDBModule : AbpModule
    {
    }
}

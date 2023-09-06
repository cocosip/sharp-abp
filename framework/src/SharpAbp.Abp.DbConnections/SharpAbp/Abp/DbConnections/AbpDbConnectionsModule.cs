using SharpAbp.Abp.Data;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections
{
    [DependsOn(
        typeof(SharpAbpDataModule)
        )]
    public class AbpDbConnectionsModule : AbpModule
    {

    }
}

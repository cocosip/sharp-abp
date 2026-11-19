using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.SqlBuilder.OceanBase
{
    /// <summary>
    /// OceanBase SQL dialect module. It replaces the default mode detector with one that reads the
    /// tenant compatibility mode from the OceanBase connection, so the connector dependency is
    /// isolated in this module.
    /// </summary>
    [DependsOn(typeof(AbpDataSqlBuilderModule))]
    public class AbpDataSqlBuilderOceanBaseModule : AbpModule
    {

    }
}

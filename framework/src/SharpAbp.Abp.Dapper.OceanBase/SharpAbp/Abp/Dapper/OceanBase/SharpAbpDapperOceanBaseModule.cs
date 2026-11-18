using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Dapper.OceanBase
{
    /// <summary>
    /// Registers the OceanBase Dapper mappings. Both tenant modes are registered at once and selected per
    /// parameter: the driver stamps each parameter with the compatibility mode of the connection that created it,
    /// so Oracle-compatible and MySQL-compatible tenants can be used in the same process.
    /// <para>
    /// LOB columns are deliberately not mapped: the driver returns an <c>OceanBaseLob</c> locator that may only be
    /// opened after its originating reader is consumed, which a Dapper type handler cannot do. Query such columns
    /// as <c>OceanBaseLob</c> and read them after the reader is done.
    /// </para>
    /// </summary>
    [DependsOn(typeof(SharpAbpDapperModule))]
    public class SharpAbpDapperOceanBaseModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<SharpAbpDapperOptions>(options =>
            {
                options.TypeMappings.Add(new OceanBaseOracleGuidTypeMapping());
                options.TypeMappings.Add(new OceanBaseMySqlGuidTypeMapping());
                options.TypeMappings.Add(new OceanBaseDecimalTypeMapping());
                options.TypeMappings.Add(new OceanBaseTimestampTypeMapping());
                options.TypeMappings.Add(new OceanBaseTimestampWithTimeZoneTypeMapping());
            });

            return Task.CompletedTask;
        }
    }
}

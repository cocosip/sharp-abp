using SharpAbp.Abp.Data.SqlBuilder.OceanBase;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Tests that the OceanBase module replaces the default mode detector
    /// </summary>
    public class OceanBaseSqlBuilderModuleTests : AbpIntegratedTest<OceanBaseSqlBuilderTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        [Fact]
        public void Module_ShouldRegisterConnectorModeDetector()
        {
            var detector = GetRequiredService<IOceanBaseDatabaseModeDetector>();

            Assert.IsType<OceanBaseDatabaseModeDetector>(detector);
        }
    }

    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpDataSqlBuilderOceanBaseModule),
        typeof(AbpTestBaseModule)
        )]
    public class OceanBaseSqlBuilderTestModule : AbpModule
    {

    }
}

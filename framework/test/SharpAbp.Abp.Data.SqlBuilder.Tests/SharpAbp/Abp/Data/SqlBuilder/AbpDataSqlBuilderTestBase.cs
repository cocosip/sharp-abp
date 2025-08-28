using Volo.Abp;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Base class for SqlBuilder module tests
    /// </summary>
    public abstract class AbpDataSqlBuilderTestBase : AbpIntegratedTest<AbpDataSqlBuilderTestModule>
    {
        /// <summary>
        /// Configure ABP application creation options
        /// </summary>
        /// <param name="options">Application creation options</param>
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
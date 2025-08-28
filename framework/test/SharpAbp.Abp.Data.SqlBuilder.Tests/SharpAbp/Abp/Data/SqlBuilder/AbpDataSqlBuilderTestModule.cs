using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Test module for SqlBuilder functionality
    /// </summary>
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpDataSqlBuilderModule),
        typeof(AbpTestBaseModule)
        )]
    public class AbpDataSqlBuilderTestModule : AbpModule
    {
        /// <summary>
        /// Configure services for the test module
        /// </summary>
        /// <param name="context">The service configuration context</param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<DataSqlBuilderOptions>(options =>
            {
                // Add test SQL parameter conversion contributors
                options.SqlParamConversionContributors.Add<StringEntitySqlParamConversionContributor>();
                options.SqlParamConversionContributors.Add<IntegerEntitySqlParamConversionContributor>();
                options.SqlParamConversionContributors.Add<DateTimeEntitySqlParamConversionContributor>();
                options.SqlParamConversionContributors.Add<GenericObjectSqlParamConversionContributor>();
                options.SqlParamConversionContributors.Add<NullReturningSqlParamConversionContributor>();
            });
        }
    }
}
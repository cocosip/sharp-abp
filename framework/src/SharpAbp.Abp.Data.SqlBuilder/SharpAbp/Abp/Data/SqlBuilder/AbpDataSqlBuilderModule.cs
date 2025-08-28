using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    [DependsOn(
        typeof(SharpAbpDataModule)
        )]
    public class AbpDataSqlBuilderModule : AbpModule
    {

        /// <summary>
        /// Configure services for the SqlBuilder module
        /// </summary>
        /// <param name="context">The service configuration context</param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // // Register database dialect adapters as keyed services
            // context.Services.AddKeyedTransient<IDatabaseDialectAdapter, SqlServerDialectAdapter>(DatabaseProvider.SqlServer);
            // context.Services.AddKeyedTransient<IDatabaseDialectAdapter, MySqlDialectAdapter>(DatabaseProvider.MySql);
            // context.Services.AddKeyedTransient<IDatabaseDialectAdapter, OracleDialectAdapter>(DatabaseProvider.Oracle);
            // context.Services.AddKeyedTransient<IDatabaseDialectAdapter, GaussDBDialectAdapter>(DatabaseProvider.GaussDB);
            // context.Services.AddKeyedTransient<IDatabaseDialectAdapter, OpenGaussDialectAdapter>(DatabaseProvider.OpenGauss);
            // context.Services.AddKeyedTransient<IDatabaseDialectAdapter, DmDialectAdapter>(DatabaseProvider.Dm);

            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<DataSqlBuilderOptions>(options => { });
            return Task.CompletedTask;
        }
    }
}
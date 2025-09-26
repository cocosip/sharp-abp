using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Dapper.Oracle;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.EntityFrameworkCore;
using Volo.Abp.Dapper;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Dapper
{
    [DependsOn(
        typeof(SharpAbpEntityFrameworkCoreModule),
        typeof(AbpDapperModule)
        )]
    public class SharpAbpDapperModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            var databaseProvider = configuration.GetDatabaseProvider();
            if (databaseProvider == DatabaseProvider.Oracle)
            {
                DapperOracleExtensions.ConfigureOracleTypeHandlers();
            }

            return Task.CompletedTask;
        }

    }
}

using System;
using System.Threading.Tasks;
using Dapper;
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
                SqlMapper.ResetTypeHandlers();

                SqlMapper.RemoveTypeMap(typeof(Guid));
                SqlMapper.RemoveTypeMap(typeof(Guid?));

                SqlMapper.RemoveTypeMap(typeof(bool));
                SqlMapper.RemoveTypeMap(typeof(bool?));

                SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHandler());
                SqlMapper.AddTypeHandler(typeof(Guid?), new NullableGuidTypeHandler());

                SqlMapper.AddTypeHandler(typeof(bool), new BoolTypeHandler());
                SqlMapper.AddTypeHandler(typeof(bool?), new NullableBoolTypeHandler());
            }

            return Task.CompletedTask;
        }

    }
}

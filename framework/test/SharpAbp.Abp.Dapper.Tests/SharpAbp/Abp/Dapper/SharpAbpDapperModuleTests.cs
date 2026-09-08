using System;
using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using SharpAbp.Abp.Dapper.DM;
using SharpAbp.Abp.Dapper.GaussDB;
using SharpAbp.Abp.Dapper.MySQL;
using SharpAbp.Abp.Dapper.Oracle;
using SharpAbp.Abp.Dapper.PostgreSql;
using SharpAbp.Abp.Dapper.Sqlite;
using SharpAbp.Abp.Dapper.SqlServer;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Data;
using Xunit;

namespace SharpAbp.Abp.Dapper
{
    [Collection(DapperTypeHandlerCollection.Name)]
    public sealed class SharpAbpDapperModuleTests : AbpIntegratedTest<SharpAbpDapperTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        [Fact]
        public void Modules_Should_Automatically_Register_All_Provider_Mappings()
        {
            var guid = Guid.Parse("ed40e2a8-d14f-4462-9043-647e25e48936");
            var handler = GetRequiredTypeHandler(typeof(Guid));

            var oracleParameter = new OracleParameter();
            handler.SetValue(oracleParameter, guid);
            Assert.Equal(guid.ToByteArray(), oracleParameter.Value);

            var defaultParameter = new TestDbParameter();
            handler.SetValue(defaultParameter, guid);
            Assert.Equal(guid, defaultParameter.Value);
            Assert.Equal(DbType.Guid, defaultParameter.DbType);

            var postgreSqlParameter = new NpgsqlParameter();
            handler.SetValue(postgreSqlParameter, guid);
            Assert.Equal(NpgsqlDbType.Uuid, postgreSqlParameter.NpgsqlDbType);
            Assert.Equal(guid, postgreSqlParameter.Value);
            Assert.Equal(guid, handler.Parse(typeof(Guid), guid));

            var nullableGuidHandler = GetRequiredTypeHandler(typeof(Guid?));
            Assert.Same(handler, nullableGuidHandler);
            var nullablePostgreSqlParameter = new NpgsqlParameter();
            nullableGuidHandler.SetValue(nullablePostgreSqlParameter, null!);
            Assert.Equal(NpgsqlDbType.Uuid, nullablePostgreSqlParameter.NpgsqlDbType);
            Assert.Equal(DBNull.Value, nullablePostgreSqlParameter.Value);

            Assert.Null(handler.Parse(typeof(Guid?), DBNull.Value));
            Assert.Equal(Guid.Empty, handler.Parse(typeof(Guid), DBNull.Value));

            var nullableOracleParameter = new OracleParameter();
            handler.SetValue(nullableOracleParameter, null!);
            Assert.Equal(DBNull.Value, nullableOracleParameter.Value);
            Assert.Equal(DbType.Binary, nullableOracleParameter.DbType);
            Assert.Equal(16, nullableOracleParameter.Size);

            var boolHandler = GetRequiredTypeHandler(typeof(bool?));
            Assert.Same(GetRequiredTypeHandler(typeof(bool)), boolHandler);
            Assert.Null(boolHandler.Parse(typeof(bool?), DBNull.Value));
            Assert.Equal(false, boolHandler.Parse(typeof(bool), DBNull.Value));

            var oracleBoolParameter = new OracleParameter();
            boolHandler.SetValue(oracleBoolParameter, true);
            Assert.Equal((byte)1, oracleBoolParameter.Value);

            var defaultBoolParameter = new TestDbParameter();
            boolHandler.SetValue(defaultBoolParameter, true);
            Assert.Equal(true, defaultBoolParameter.Value);
            Assert.Equal(DbType.Boolean, defaultBoolParameter.DbType);

            var extraPropertiesHandler = GetRequiredTypeHandler(typeof(ExtraPropertyDictionary));
            var extraProperties = Assert.IsType<ExtraPropertyDictionary>(
                extraPropertiesHandler.Parse(typeof(ExtraPropertyDictionary), DBNull.Value));
            Assert.Empty(extraProperties);
        }

#pragma warning disable CS0618
        private static SqlMapper.ITypeHandler GetRequiredTypeHandler(Type type)
        {
            SqlMapper.LookupDbType(type, "value", false, out var handler);
            return Assert.IsAssignableFrom<SqlMapper.ITypeHandler>(handler);
        }
#pragma warning restore CS0618
    }

    [DependsOn(
        typeof(SharpAbpDapperOracleModule),
        typeof(SharpAbpDapperPostgreSqlModule),
        typeof(SharpAbpDapperMySQLModule),
        typeof(SharpAbpDapperSqlServerModule),
        typeof(SharpAbpDapperSqliteModule),
        typeof(SharpAbpDapperDmModule),
        typeof(SharpAbpDapperGaussDBModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule))]
    public sealed class SharpAbpDapperTestModule : AbpModule
    {
    }
}

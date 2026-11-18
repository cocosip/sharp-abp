using System;
using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using OceanBaseConnector;
using Oracle.ManagedDataAccess.Client;
using SharpAbp.Abp.Dapper.DM;
using SharpAbp.Abp.Dapper.GaussDB;
using SharpAbp.Abp.Dapper.MySQL;
using SharpAbp.Abp.Dapper.OceanBase;
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

            var oceanBaseOracleParameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.Oracle);
            handler.SetValue(oceanBaseOracleParameter, guid);
            Assert.Equal(DbType.Binary, oceanBaseOracleParameter.DbType);
            Assert.Equal(16, oceanBaseOracleParameter.Size);
            Assert.Equal(guid.ToByteArray(), oceanBaseOracleParameter.Value);

            var oceanBaseMySqlParameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.MySql);
            handler.SetValue(oceanBaseMySqlParameter, guid);
            Assert.Equal(DbType.String, oceanBaseMySqlParameter.DbType);
            Assert.Equal(36, oceanBaseMySqlParameter.Size);
            Assert.Equal(guid.ToString("D"), oceanBaseMySqlParameter.Value);

            Assert.Equal(guid, handler.Parse(typeof(Guid), guid.ToString("D")));

            var decimalHandler = GetRequiredTypeHandler(typeof(decimal));
            Assert.Same(GetRequiredTypeHandler(typeof(decimal?)), decimalHandler);
            Assert.Equal(1234.5678m, decimalHandler.Parse(typeof(decimal), new OceanBaseDecimal(1234.5678m)));
            Assert.Equal(1234.5678m, decimalHandler.Parse(typeof(decimal), 1234.5678m));

            var dateTimeHandler = GetRequiredTypeHandler(typeof(DateTime));
            Assert.Same(GetRequiredTypeHandler(typeof(DateTime?)), dateTimeHandler);
            var timestamp = new OceanBaseTimestamp(new DateTime(2024, 1, 2, 3, 4, 5));
            Assert.Equal(new DateTime(2024, 1, 2, 3, 4, 5), dateTimeHandler.Parse(typeof(DateTime), timestamp));
            Assert.Equal(
                new DateTime(2024, 1, 2, 3, 4, 5),
                dateTimeHandler.Parse(typeof(DateTime), new OceanBaseTimestampLocalTimeZone(timestamp, "+08:00")));
            Assert.Null(dateTimeHandler.Parse(typeof(DateTime?), DBNull.Value));

            var dateTimeOffsetHandler = GetRequiredTypeHandler(typeof(DateTimeOffset));
            var dateTimeOffset = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.FromHours(8));
            Assert.Equal(
                dateTimeOffset,
                dateTimeOffsetHandler.Parse(
                    typeof(DateTimeOffset),
                    new OceanBaseTimestampWithTimeZone(dateTimeOffset)));

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

            // OceanBase has no bool mapping: the driver binds DbType.Boolean as 1/0 in both tenant modes,
            // so the Dapper default path must leave the value untouched.
            foreach (var mode in new[] { OceanBaseCompatibilityMode.Oracle, OceanBaseCompatibilityMode.MySql })
            {
                var oceanBaseBoolParameter = OceanBaseTestParameters.Create(mode);
                boolHandler.SetValue(oceanBaseBoolParameter, true);
                Assert.Equal(true, oceanBaseBoolParameter.Value);
                Assert.Equal(DbType.Boolean, oceanBaseBoolParameter.DbType);
            }

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
        typeof(SharpAbpDapperOceanBaseModule),
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

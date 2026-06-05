using Microsoft.Data.SqlClient;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.DbConnections;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DbConnectionFactoryTest : DbConnectionsManagementTestBase
    {
        private readonly IDatabaseConnectionInfoAppService _databaseConnectionInfoAppService;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ICurrentTenant _currentTenant;
        public DbConnectionFactoryTest()
        {
            _databaseConnectionInfoAppService = GetRequiredService<IDatabaseConnectionInfoAppService>();
            _dbConnectionFactory = GetRequiredService<IDbConnectionFactory>();
            _currentTenant = GetRequiredService<ICurrentTenant>();
        }

        [Fact]
        public async Task GetDbConnectionInfoAsync_Test()
        {
            var mysqlId = await _databaseConnectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto()
            {
                Name = "mysql0",
                DatabaseProvider = DatabaseProvider.MySql.ToString(),
                ConnectionString = "Server=localhost;Port=3306;Database=demo1;User=root;"
            });

            var pgId = await _databaseConnectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto()
            {
                Name = "postgresql1",
                DatabaseProvider = DatabaseProvider.PostgreSql.ToString(),
                ConnectionString = "Server=localhost;Port=9432;Username=root;Password=123456;Database=pg;"
            });

            var mysqlConnectionInfo = await _dbConnectionFactory.GetDbConnectionInfoAsync("mysql0");
            Assert.Equal("Server=localhost;Port=3306;Database=demo1;User=root;", mysqlConnectionInfo.ConnectionString);
            Assert.Equal(DatabaseProvider.MySql, mysqlConnectionInfo.DatabaseProvider);

            var postgresqlConnectionInfo = await _dbConnectionFactory.GetDbConnectionInfoAsync("postgresql1");
            Assert.Equal("Server=localhost;Port=9432;Username=root;Password=123456;Database=pg;", postgresqlConnectionInfo.ConnectionString);
            Assert.Equal(DatabaseProvider.PostgreSql, postgresqlConnectionInfo.DatabaseProvider);

            await Assert.ThrowsAsync<AbpException>(() =>
            {
                return _dbConnectionFactory.GetDbConnectionInfoAsync("oracle4");
            });
        }

        [Fact]
        public async Task GetDbConnectionAsync_Test()
        {
            var sqlServerId = await _databaseConnectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto()
            {
                Name = "sqlserver3",
                DatabaseProvider = DatabaseProvider.SqlServer.ToString(),
                ConnectionString = "server=.;database=demo3;"
            });

            var sqlConnection = await _dbConnectionFactory.GetDbConnectionAsync("sqlserver3");
            Assert.Equal("server=.;database=demo3;", sqlConnection.ConnectionString);
            Assert.Equal(typeof(SqlConnection), sqlConnection.GetType());

            await Assert.ThrowsAsync<AbpException>(() =>
            {
                return _dbConnectionFactory.GetDbConnectionAsync("sqlite5");
            });

        }

        [Fact]
        public async Task Update_Should_Clear_Global_Cache_When_Connection_Is_Read_Under_Tenant()
        {
            var connectionInfo = await _databaseConnectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto()
            {
                Name = "tenant-cache-update",
                DatabaseProvider = DatabaseProvider.MySql.ToString(),
                ConnectionString = "Server=old;"
            });

            var tenantId = Guid.NewGuid();

            using (_currentTenant.Change(tenantId))
            {
                var cached = await _dbConnectionFactory.GetDbConnectionInfoAsync("tenant-cache-update");
                Assert.Equal("Server=old;", cached.ConnectionString);
            }

            await _databaseConnectionInfoAppService.UpdateAsync(connectionInfo.Id, new UpdateDatabaseConnectionInfoDto()
            {
                Name = "tenant-cache-update",
                DatabaseProvider = DatabaseProvider.MySql.ToString(),
                ConnectionString = "Server=new;"
            });

            using (_currentTenant.Change(tenantId))
            {
                var refreshed = await _dbConnectionFactory.GetDbConnectionInfoAsync("tenant-cache-update");
                Assert.Equal("Server=new;", refreshed.ConnectionString);
            }
        }

    }
}

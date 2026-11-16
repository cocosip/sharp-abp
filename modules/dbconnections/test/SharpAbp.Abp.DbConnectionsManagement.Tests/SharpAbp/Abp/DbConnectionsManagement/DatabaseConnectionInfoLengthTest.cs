using Microsoft.EntityFrameworkCore;
using SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Volo.Abp.Validation;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoLengthTest : DbConnectionsManagementTestBase
    {
        private readonly IDatabaseConnectionInfoAppService _databaseConnectionInfoAppService;

        public DatabaseConnectionInfoLengthTest()
        {
            _databaseConnectionInfoAppService = GetRequiredService<IDatabaseConnectionInfoAppService>();
        }

        [Fact]
        public async Task Long_ConnectionString_Is_Accepted()
        {
            // A real connection string is easily longer than the old 256 character limit.
            var connectionString = "Server=127.0.0.1;Port=3306;Database=demo1;User=root;Password=" + new string('a', 700) + ";";

            var created = await _databaseConnectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto
            {
                Name = "long-connection",
                DatabaseProvider = "MySql",
                ConnectionString = connectionString
            });

            var queried = await _databaseConnectionInfoAppService.GetAsync(created.Id);
            Assert.Equal(connectionString, queried.ConnectionString);
        }

        [Fact]
        public async Task ConnectionString_Above_MaxLength_Is_Rejected()
        {
            var connectionString = new string('a', DatabaseConnectionInfoConsts.MaxConnectionStringLength + 1);

            await Assert.ThrowsAsync<AbpValidationException>(() => _databaseConnectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto
            {
                Name = "too-long-connection",
                DatabaseProvider = "MySql",
                ConnectionString = connectionString
            }));
        }

        [Fact]
        public void Configured_String_Lengths_Fit_Oracle()
        {
            var options = new DbContextOptionsBuilder<DbConnectionsManagementDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new DbConnectionsManagementDbContext(options);

            var entityType = dbContext.Model.FindEntityType(typeof(DatabaseConnectionInfo));
            Assert.NotNull(entityType);

            Assert.Equal(
                DatabaseConnectionInfoConsts.MaxNameLength,
                entityType.FindProperty(nameof(DatabaseConnectionInfo.Name)).GetMaxLength());
            Assert.Equal(
                DatabaseConnectionInfoConsts.MaxDatabaseProviderLength,
                entityType.FindProperty(nameof(DatabaseConnectionInfo.DatabaseProvider)).GetMaxLength());

            // Oracle maps a string column above 2000 to a LOB, so the connection string must stay at or below 2000.
            var connectionStringMaxLength = entityType.FindProperty(nameof(DatabaseConnectionInfo.ConnectionString)).GetMaxLength();
            Assert.Equal(DatabaseConnectionInfoConsts.MaxConnectionStringLength, connectionStringMaxLength);
            Assert.True(connectionStringMaxLength <= 2000);
        }
    }
}

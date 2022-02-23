using SharpAbp.Abp.DbConnections;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoAppServiceTest : DbConnectionsManagementTestBase
    {
        private readonly IDatabaseConnectionInfoAppService _databaseConnectionInfoAppService;
        private readonly IDatabaseConnectionInfoCacheManager _databaseConnectionInfoCacheManager;
        public DatabaseConnectionInfoAppServiceTest()
        {
            _databaseConnectionInfoAppService = GetRequiredService<IDatabaseConnectionInfoAppService>();
            _databaseConnectionInfoCacheManager = GetRequiredService<IDatabaseConnectionInfoCacheManager>();
        }

        [Fact]
        public async Task Create_Update_Delete_Test()
        {
            var d1 = await _databaseConnectionInfoAppService.CreateAsync(new CreateDatabaseConnectionInfoDto()
            {
                Name = "mysql",
                ConnectionString = "Server=127.0.0.1;Port=3306;Database=demo1;User=root;Charset=utf8;",
                DatabaseProvider = DatabaseProvider.MySql.ToString()
            });

            var databaseConnectionInfo1 = await _databaseConnectionInfoAppService.GetAsync(d1.Id);
            Assert.Equal("mysql", databaseConnectionInfo1.Name);
            Assert.Equal("MySql", databaseConnectionInfo1.DatabaseProvider);
            Assert.Equal("Server=127.0.0.1;Port=3306;Database=demo1;User=root;Charset=utf8;", databaseConnectionInfo1.ConnectionString);

            var databaseConnectionInfo1_1 = await _databaseConnectionInfoAppService.FindByNameAsync("mysql");
            Assert.Equal(databaseConnectionInfo1.Name, databaseConnectionInfo1_1.Name);
            Assert.Equal(databaseConnectionInfo1.DatabaseProvider, databaseConnectionInfo1_1.DatabaseProvider);
            Assert.Equal(databaseConnectionInfo1.ConnectionString, databaseConnectionInfo1_1.ConnectionString);

            var c0 = await _databaseConnectionInfoCacheManager.GetCacheAsync("mysql");
            Assert.Equal("mysql", c0.Name);

            await _databaseConnectionInfoAppService.UpdateAsync(d1.Id, new UpdateDatabaseConnectionInfoDto()
            {
                Name = "postgresql",
                DatabaseProvider = "PostgreSql",
                ConnectionString = "Server=127.0.0.1;Port=9432;Username=root;Password=123456;Database=demo2;"
            });

            var databaseConnectionInfo2 = await _databaseConnectionInfoAppService.GetAsync(d1.Id);
            Assert.Equal("postgresql", databaseConnectionInfo2.Name);
            Assert.Equal("PostgreSql", databaseConnectionInfo2.DatabaseProvider);
            Assert.Equal("Server=127.0.0.1;Port=9432;Username=root;Password=123456;Database=demo2;", databaseConnectionInfo2.ConnectionString);

            var databaseConnectionInfo2_1 = await _databaseConnectionInfoAppService.FindByNameAsync("postgresql");
            Assert.Equal(databaseConnectionInfo2.Name, databaseConnectionInfo2_1.Name);
            Assert.Equal(databaseConnectionInfo2.DatabaseProvider, databaseConnectionInfo2_1.DatabaseProvider);
            Assert.Equal(databaseConnectionInfo2.ConnectionString, databaseConnectionInfo2_1.ConnectionString);

            var databaseConnectionInfo2_2 = await _databaseConnectionInfoAppService.FindByNameAsync("postgresql");
            Assert.NotNull(databaseConnectionInfo2_2);


            var c1 = await _databaseConnectionInfoCacheManager.GetCacheAsync("mysql");
            Assert.NotNull(c1);

            var c2 = await _databaseConnectionInfoCacheManager.GetCacheAsync("postgresql");
            Assert.NotNull(c2);

        }
    }
}

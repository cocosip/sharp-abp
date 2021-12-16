using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseProviderAppServiceTest : DbConnectionsManagementTestBase
    {
        private readonly IDatabaseProviderAppService _databaseProviderAppService;
        public DatabaseProviderAppServiceTest()
        {
            _databaseProviderAppService = GetRequiredService<IDatabaseProviderAppService>();
        }

        [Fact]
        public async Task GetAllAsync_Test()
        {
            var databaseProviders = await _databaseProviderAppService.GetAllAsync();
            Assert.Equal(4, databaseProviders.Count);
            Assert.Contains("MySql", databaseProviders);
            Assert.Contains("PostgreSql", databaseProviders);
            Assert.Contains("SqlServer", databaseProviders);
           //Assert.Contains("Oracle", databaseProviders);
            Assert.Contains("Sqlite", databaseProviders);
        }

    }
}

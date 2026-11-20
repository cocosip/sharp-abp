using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OceanBaseConnector;
using SharpAbp.Abp.Data;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class OceanBaseDbConnectionTest : AbpDbConnectionsTestBase
    {
        [Theory]
        [InlineData("MySql")]
        [InlineData("Oracle")]
        public async Task Creator_Preserves_Connection_Settings_And_Does_Not_Open(string mode)
        {
            var connectionString = $"Server=localhost;Port=2881;User ID=app;Database=appdb;ExpectedMode={mode}";
            using var connection = await GetRequiredService<IDbConnectionCreateService>().CreateAsync(
                new DbConnectionInfo(DatabaseProvider.OceanBase, connectionString));

            Assert.IsType<OceanBaseConnection>(connection);
            Assert.Equal(ConnectionState.Closed, connection.State);
            var settings = new OceanBaseConnectionStringBuilder(connection.ConnectionString);
            Assert.Equal(mode, settings.ExpectedMode.ToString());
            Assert.Equal("appdb", settings.Database);
        }

        [Fact]
        public async Task Named_Connection_Is_Resolved_By_Existing_Factory()
        {
            var options = GetRequiredService<IOptions<AbpDbConnectionsOptions>>().Value;
            Assert.Contains(DatabaseProvider.OceanBase, options.DatabaseProviders);
            options.DbConnections.Configure("OceanBaseReporting", connection =>
            {
                connection.DatabaseProvider = DatabaseProvider.OceanBase;
                connection.ConnectionString = "Server=localhost;Port=2881;User ID=report;Database=reports";
            });

            var factory = GetRequiredService<IDbConnectionFactory>();
            using var connection = await factory.GetDbConnectionAsync("OceanBaseReporting");
            Assert.IsType<OceanBaseConnection>(connection);
            Assert.Equal("reports", connection.Database);
            Assert.Equal(DatabaseProvider.OceanBase, (await factory.GetDbConnectionInfoAsync("OceanBaseReporting")).DatabaseProvider);
        }

        [Fact]
        public async Task Invalid_Connection_String_Rejects_Before_Opening()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => GetRequiredService<IDbConnectionCreateService>().CreateAsync(
                new DbConnectionInfo(DatabaseProvider.OceanBase, "Port=not-a-number")));
        }
    }
}

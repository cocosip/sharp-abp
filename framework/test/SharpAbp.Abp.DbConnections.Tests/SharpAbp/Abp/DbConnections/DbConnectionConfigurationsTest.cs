using SharpAbp.Abp.Data;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionConfigurationsTest
    {
        [Fact]
        public void Should_Have_Default_Configuration()
        {
            var configurations = new DbConnectionConfigurations();

            var configuration = configurations.GetConfiguration("missing");

            Assert.Equal(DatabaseProvider.InMemory, configuration.DatabaseProvider);
            Assert.Equal(string.Empty, configuration.ConnectionString);
        }

        [Fact]
        public void ConfigureDefault_Should_Update_Default_And_Fallback_Configuration()
        {
            var configurations = new DbConnectionConfigurations();

            configurations.ConfigureDefault(connection =>
            {
                connection.DatabaseProvider = DatabaseProvider.SqlServer;
                connection.ConnectionString = "Server=.;Database=demo;";
            });

            var defaultConfiguration = configurations.GetConfiguration<DefaultDbConnection>();
            var fallbackConfiguration = configurations.GetConfiguration("unknown");

            Assert.Equal(DatabaseProvider.SqlServer, defaultConfiguration.DatabaseProvider);
            Assert.Equal("Server=.;Database=demo;", defaultConfiguration.ConnectionString);
            Assert.Equal(defaultConfiguration.DatabaseProvider, fallbackConfiguration.DatabaseProvider);
            Assert.Equal(defaultConfiguration.ConnectionString, fallbackConfiguration.ConnectionString);
        }

        [Fact]
        public void Configure_Should_Use_Attribute_Name_For_Typed_Connection()
        {
            var configurations = new DbConnectionConfigurations();

            configurations.Configure<ReportingDbConnection>(connection =>
            {
                connection.DatabaseProvider = DatabaseProvider.PostgreSql;
                connection.ConnectionString = "Host=localhost;";
            });

            var configuration = configurations.GetConfiguration("reporting");

            Assert.Equal(DatabaseProvider.PostgreSql, configuration.DatabaseProvider);
            Assert.Equal("Host=localhost;", configuration.ConnectionString);
        }

        [Fact]
        public void ConfigureAll_Should_Visit_Each_Configured_Connection()
        {
            var configurations = new DbConnectionConfigurations();
            var count = 0;

            configurations.Configure("db1", connection => connection.ConnectionString = "db1");
            configurations.Configure("db2", connection => connection.ConnectionString = "db2");

            configurations.ConfigureAll((_, _) => count++);

            Assert.Equal(3, count);
        }

        [DbConnectionName("reporting")]
        private class ReportingDbConnection
        {
        }
    }
}

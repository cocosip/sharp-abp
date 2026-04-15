using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Data;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class AbpDbConnectionsOptionsTest
    {
        [Fact]
        public void Configure_Should_Read_DbConnections_And_DatabaseProviders()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["DbConnectionsOptions:DbConnections:primary:DatabaseProvider"] = "SqlServer",
                    ["DbConnectionsOptions:DbConnections:primary:ConnectionString"] = "Server=.;Database=primary;",
                    ["DbConnectionsOptions:DbConnections:audit:DatabaseProvider"] = "MySql",
                    ["DbConnectionsOptions:DbConnections:audit:ConnectionString"] = "Server=127.0.0.1;Database=audit;",
                    ["DbConnectionsOptions:DatabaseProviders:0"] = "SqlServer",
                    ["DbConnectionsOptions:DatabaseProviders:1"] = "SqlServer",
                    ["DbConnectionsOptions:DatabaseProviders:2"] = "MySql"
                })
                .Build();

            var options = new AbpDbConnectionsOptions();
            options.Configure(configuration);

            Assert.Equal(DatabaseProvider.SqlServer, options.DbConnections.GetConfiguration("primary").DatabaseProvider);
            Assert.Equal("Server=.;Database=primary;", options.DbConnections.GetConfiguration("primary").ConnectionString);
            Assert.Equal(DatabaseProvider.MySql, options.DbConnections.GetConfiguration("audit").DatabaseProvider);
            Assert.Equal("Server=127.0.0.1;Database=audit;", options.DbConnections.GetConfiguration("audit").ConnectionString);
            Assert.Equal(2, options.DatabaseProviders.Count);
            Assert.Contains(DatabaseProvider.SqlServer, options.DatabaseProviders);
            Assert.Contains(DatabaseProvider.MySql, options.DatabaseProviders);
        }

        [Fact]
        public void Configure_Should_Keep_Defaults_When_Configuration_Is_Empty()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();

            var options = new AbpDbConnectionsOptions();
            options.Configure(configuration);

            Assert.Equal(DatabaseProvider.InMemory, options.DbConnections.GetConfiguration("missing").DatabaseProvider);
            Assert.Empty(options.DatabaseProviders);
        }
    }
}

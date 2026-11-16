using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Data;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class LargeTextColumnOptionsTests
    {
        [Fact]
        public void Providers_Can_Be_Configured_From_Configuration()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["EfCoreOptions:LargeTextColumns:Providers:MySql:StringMaxLength"] = "2000",
                    ["EfCoreOptions:LargeTextColumns:Providers:MySql:StringColumnType"] = "text"
                })
                .Build();

            var options = new AbpLargeTextColumnOptions();
            configuration.GetSection("EfCoreOptions:LargeTextColumns").Bind(options);

            Assert.True(options.Enabled);
            Assert.True(options.IncludeBinary);

            // The configured entry replaces the built in entry of that provider, the entries of the
            // other providers keep their defaults.
            Assert.Equal(2000, options.Providers[DatabaseProvider.MySql].StringMaxLength);
            Assert.Equal("text", options.Providers[DatabaseProvider.MySql].StringColumnType);
            Assert.Equal("CLOB", options.Providers[DatabaseProvider.Dm].StringColumnType);
            Assert.Equal(32767, options.Providers[DatabaseProvider.OceanBase].StringMaxLength);
        }

        [Fact]
        public void Convention_Can_Be_Disabled_From_Configuration()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["EfCoreOptions:LargeTextColumns:Enabled"] = "false"
                })
                .Build();

            var options = new AbpLargeTextColumnOptions();
            configuration.GetSection("EfCoreOptions:LargeTextColumns").Bind(options);

            Assert.False(options.Enabled);
            Assert.True(options.Providers.ContainsKey(DatabaseProvider.MySql));
        }
    }
}

using System;
using SharpAbp.Abp.Data;
using Xunit;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionInfoTest
    {
        [Fact]
        public void Equality_Uses_Provider_And_Current_Connection_String()
        {
            var info = new DbConnectionInfo(DatabaseProvider.Sqlite, "Data Source=first");
            var same = new DbConnectionInfo(DatabaseProvider.Sqlite, "Data Source=first");
            Assert.True(info.Equals(same));
            Assert.True(info.Equals((object)same));
            Assert.Equal(info.GetHashCode(), same.GetHashCode());
            Assert.False(info.Equals(new DbConnectionInfo(DatabaseProvider.MySql, info.ConnectionString)));
            Assert.False(info.Equals(new DbConnectionInfo(DatabaseProvider.Sqlite, "Data Source=second")));
            Assert.False(info.Equals((DbConnectionInfo)null));
            Assert.False(info.Equals((object)null));
            Assert.False(info.Equals("Data Source=first"));

            same.ConnectionString = "Data Source=second";
            Assert.False(info.Equals(same));
            Assert.Equal(new DbConnectionInfo().GetHashCode(), new DbConnectionInfo(DatabaseProvider.SqlServer, null).GetHashCode());
        }

        [Fact]
        public void Reconfiguration_Preserves_The_Existing_Instance_And_Other_Fields()
        {
            var configurations = new DbConnectionConfigurations();
            Assert.Same(configurations, configurations.Configure("reporting", configuration =>
                configuration.DatabaseProvider = DatabaseProvider.PostgreSql));
            var original = configurations.GetConfiguration("reporting");
            configurations.Configure("reporting", configuration => configuration.ConnectionString = "Host=reporting");

            Assert.Same(original, configurations.GetConfiguration("reporting"));
            Assert.Equal(DatabaseProvider.PostgreSql, original.DatabaseProvider);
            Assert.Equal("Host=reporting", original.ConnectionString);
            Assert.Throws<ArgumentNullException>(() => configurations.Configure("reporting", null));
            Assert.Equal("reporting", new DbConnectionNameAttribute("reporting").GetName(typeof(DbConnectionInfoTest)));
        }
    }
}

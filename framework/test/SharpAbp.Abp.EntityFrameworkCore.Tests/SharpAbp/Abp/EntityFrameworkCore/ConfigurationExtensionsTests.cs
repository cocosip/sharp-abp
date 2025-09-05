using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Data;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class ConfigurationExtensionsTests : AbpEntityFrameworkCoreTestBase
    {
        [Fact]
        public void GetDatabaseProvider_ShouldReturnConfiguredProvider_WhenValidProviderInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:DatabaseProvider", "MySql" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetDatabaseProvider();

            // Assert
            Assert.Equal(DatabaseProvider.MySql, result);
        }

        [Fact]
        public void GetDatabaseProvider_ShouldReturnDefaultProvider_WhenNoProviderInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetDatabaseProvider();

            // Assert
            Assert.Equal(DatabaseProvider.PostgreSql, result);
        }

        [Fact]
        public void GetDatabaseProvider_ShouldReturnCustomDefault_WhenSpecified()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetDatabaseProvider(DatabaseProvider.SqlServer);

            // Assert
            Assert.Equal(DatabaseProvider.SqlServer, result);
        }

        [Fact]
        public void GetDatabaseProvider_ShouldReturnDefault_WhenInvalidProviderInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:DatabaseProvider", "InvalidProvider" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetDatabaseProvider();

            // Assert
            Assert.Equal(DatabaseProvider.PostgreSql, result);
        }

        [Fact]
        public void GetProperties_ShouldReturnProperties_WhenPropertiesExistInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:PostgreSqlVersion", "13.0" },
                { "EfCoreOptions:Properties:MySqlVersion", "8.0" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetProperties();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("13.0", result["PostgreSqlVersion"]);
            Assert.Equal("8.0", result["MySqlVersion"]);
        }

        [Fact]
        public void GetProperties_ShouldReturnNull_WhenNoPropertiesInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetProperties();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPostgreSqlVersion_ShouldReturnConfiguredVersion_WhenVersionInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:PostgreSqlVersion", "14.0" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetPostgreSqlVersion();

            // Assert
            Assert.Equal("14.0", result);
        }

        [Fact]
        public void GetPostgreSqlVersion_ShouldReturnEmptyString_WhenNoVersionInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetPostgreSqlVersion();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void GetOracleSQLCompatibility_ShouldReturnConfiguredVersion_WhenValidVersionInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:OracleSQLCompatibility", "DatabaseVersion21" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetOracleSQLCompatibility();

            // Assert
            Assert.Equal("DatabaseVersion21", result);
        }

        [Fact]
        public void GetOracleSQLCompatibility_ShouldReturnDefault_WhenInvalidVersionInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:OracleSQLCompatibility", "InvalidVersion" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetOracleSQLCompatibility();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleSQLCompatibility, result);
        }

        [Fact]
        public void GetOracleSQLCompatibility_ShouldReturnDefault_WhenNoVersionInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetOracleSQLCompatibility();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleSQLCompatibility, result);
        }

        [Fact]
        public void GetOracleAllowedLogonVersionClient_ShouldReturnConfiguredVersion_WhenValidVersionInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:OracleAllowedLogonVersionClient", "Version12" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetOracleAllowedLogonVersionClient();

            // Assert
            Assert.Equal("Version12", result);
        }

        [Fact]
        public void GetOracleAllowedLogonVersionClient_ShouldReturnDefault_WhenInvalidVersionInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:OracleAllowedLogonVersionClient", "InvalidVersion" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetOracleAllowedLogonVersionClient();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleAllowedLogonVersionClient, result);
        }

        [Fact]
        public void GetOracleAllowedLogonVersionClient_ShouldReturnDefault_WhenNoVersionInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetOracleAllowedLogonVersionClient();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleAllowedLogonVersionClient, result);
        }

        [Fact]
        public void GetMySqlVersion_ShouldReturnConfiguredVersion_WhenVersionInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:MySqlVersion", "8.0" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetMySqlVersion();

            // Assert
            Assert.Equal("8.0", result);
        }

        [Fact]
        public void GetMySqlVersion_ShouldReturnDefault_WhenNoVersionInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetMySqlVersion();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.MySqlVersion, result);
        }

        [Fact]
        public void GetMySqlServerType_ShouldReturnConfiguredType_WhenTypeInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:MySqlServerType", "MariaDb" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetMySqlServerType();

            // Assert
            Assert.Equal("MariaDb", result);
        }

        [Fact]
        public void GetMySqlServerType_ShouldReturnDefault_WhenNoTypeInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetMySqlServerType();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.MySqlServerType, result);
        }

        [Fact]
        public void GetDefaultSchema_ShouldReturnConfiguredSchema_WhenSchemaInConfiguration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                { "EfCoreOptions:Properties:DefaultSchema", "custom_schema" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            // Act
            var result = configuration.GetDefaultSchema();

            // Assert
            Assert.Equal("custom_schema", result);
        }

        [Fact]
        public void GetDefaultSchema_ShouldReturnEmptyString_WhenNoSchemaInConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = configuration.GetDefaultSchema();

            // Assert
            Assert.Equal("", result);
        }
    }
}
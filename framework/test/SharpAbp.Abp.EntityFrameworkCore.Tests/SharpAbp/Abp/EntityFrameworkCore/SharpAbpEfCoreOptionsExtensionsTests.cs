using SharpAbp.Abp.Data;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class SharpAbpEfCoreOptionsExtensionsTests : AbpEntityFrameworkCoreTestBase
    {
        [Fact]
        public void GetPostgreSqlVersion_ShouldReturnConfiguredVersion_WhenVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.PostgreSqlVersion, "14.0" }
                }
            };

            // Act
            var result = options.GetPostgreSqlVersion();

            // Assert
            Assert.Equal("14.0", result);
        }

        [Fact]
        public void GetPostgreSqlVersion_ShouldReturnEmptyString_WhenNoVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions();

            // Act
            var result = options.GetPostgreSqlVersion();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void GetPostgreSqlVersion_ShouldReturnEmptyString_WhenVersionIsEmpty()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.PostgreSqlVersion, "" }
                }
            };

            // Act
            var result = options.GetPostgreSqlVersion();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void GetOracleSQLCompatibility_ShouldReturnConfiguredVersion_WhenValidVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.OracleSQLCompatibility, "DatabaseVersion21" }
                }
            };

            // Act
            var result = options.GetOracleSQLCompatibility();

            // Assert
            Assert.Equal("DatabaseVersion21", result);
        }

        [Fact]
        public void GetOracleSQLCompatibility_ShouldReturnDefault_WhenInvalidVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.OracleSQLCompatibility, "InvalidVersion" }
                }
            };

            // Act
            var result = options.GetOracleSQLCompatibility();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleSQLCompatibility, result);
        }

        [Fact]
        public void GetOracleSQLCompatibility_ShouldReturnDefault_WhenNoVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions();

            // Act
            var result = options.GetOracleSQLCompatibility();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleSQLCompatibility, result);
        }

        [Fact]
        public void GetOracleSQLCompatibility_ShouldBeCaseInsensitive()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.OracleSQLCompatibility, "databaseversion21" }
                }
            };

            // Act
            var result = options.GetOracleSQLCompatibility();

            // Assert
            Assert.Equal("databaseversion21", result);
        }

        [Fact]
        public void GetOracleAllowedLogonVersionClient_ShouldReturnConfiguredVersion_WhenValidVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.OracleAllowedLogonVersionClient, "Version12" }
                }
            };

            // Act
            var result = options.GetOracleAllowedLogonVersionClient();

            // Assert
            Assert.Equal("Version12", result);
        }

        [Fact]
        public void GetOracleAllowedLogonVersionClient_ShouldReturnDefault_WhenInvalidVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.OracleAllowedLogonVersionClient, "InvalidVersion" }
                }
            };

            // Act
            var result = options.GetOracleAllowedLogonVersionClient();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleAllowedLogonVersionClient, result);
        }

        [Fact]
        public void GetOracleAllowedLogonVersionClient_ShouldReturnDefault_WhenNoVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions();

            // Act
            var result = options.GetOracleAllowedLogonVersionClient();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.OracleAllowedLogonVersionClient, result);
        }

        [Fact]
        public void GetOracleAllowedLogonVersionClient_ShouldBeCaseInsensitive()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.OracleAllowedLogonVersionClient, "version12" }
                }
            };

            // Act
            var result = options.GetOracleAllowedLogonVersionClient();

            // Assert
            Assert.Equal("version12", result);
        }

        [Fact]
        public void GetMySqlVersion_ShouldReturnConfiguredVersion_WhenVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.MySqlVersion, "8.0" }
                }
            };

            // Act
            var result = options.GetMySqlVersion();

            // Assert
            Assert.Equal("8.0", result);
        }

        [Fact]
        public void GetMySqlVersion_ShouldReturnDefault_WhenNoVersionInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions();

            // Act
            var result = options.GetMySqlVersion();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.MySqlVersion, result);
        }

        [Fact]
        public void GetMySqlVersion_ShouldReturnDefault_WhenVersionIsEmpty()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.MySqlVersion, "" }
                }
            };

            // Act
            var result = options.GetMySqlVersion();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.MySqlVersion, result);
        }

        [Fact]
        public void GetMySqlServerType_ShouldReturnConfiguredType_WhenTypeInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.MySqlServerType, "MariaDb" }
                }
            };

            // Act
            var result = options.GetMySqlServerType();

            // Assert
            Assert.Equal("MariaDb", result);
        }

        [Fact]
        public void GetMySqlServerType_ShouldReturnDefault_WhenNoTypeInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions();

            // Act
            var result = options.GetMySqlServerType();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.MySqlServerType, result);
        }

        [Fact]
        public void GetMySqlServerType_ShouldReturnDefault_WhenTypeIsEmpty()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.MySqlServerType, "" }
                }
            };

            // Act
            var result = options.GetMySqlServerType();

            // Assert
            Assert.Equal(EfCoreConstants.DefaultValues.MySqlServerType, result);
        }

        [Fact]
        public void GetDefaultSchema_ShouldReturnConfiguredSchema_WhenSchemaInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.DefaultSchema, "custom_schema" }
                }
            };

            // Act
            var result = options.GetDefaultSchema();

            // Assert
            Assert.Equal("custom_schema", result);
        }

        [Fact]
        public void GetDefaultSchema_ShouldReturnEmptyString_WhenNoSchemaInProperties()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions();

            // Act
            var result = options.GetDefaultSchema();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void GetDefaultSchema_ShouldReturnEmptyString_WhenSchemaIsEmpty()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.DefaultSchema, "" }
                }
            };

            // Act
            var result = options.GetDefaultSchema();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void GetDefaultSchema_ShouldReturnEmptyString_WhenSchemaIsWhitespace()
        {
            // Arrange
            var options = new SharpAbpEfCoreOptions
            {
                Properties = new Dictionary<string, string>
                {
                    { EfCoreConstants.PropertyNames.DefaultSchema, "   " }
                }
            };

            // Act
            var result = options.GetDefaultSchema();

            // Assert
            Assert.Equal("", result);
        }
    }
}
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for MySqlDialectAdapter
    /// Tests MySQL-specific database naming conventions and SQL syntax adaptations
    /// </summary>
    public class MySqlDialectAdapterTests : AbpDataSqlBuilderTestBase
    {
        private readonly IDatabaseDialectAdapter _adapter;
        private readonly Mock<IDbConnection> _mockConnection;

        /// <summary>
        /// Initializes a new instance of the MySqlDialectAdapterTests class
        /// </summary>
        public MySqlDialectAdapterTests()
        {
            _adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.MySql);
            _mockConnection = new Mock<IDbConnection>();
        }

        /// <summary>
        /// Test that DatabaseProvider property returns MySql
        /// </summary>
        [Fact]
        public void DatabaseProvider_ShouldReturnMySql()
        {
            // Act
            var provider = _adapter.DatabaseProvider;

            // Assert
            Assert.Equal(DatabaseProvider.MySql, provider);
        }

        /// <summary>
        /// Test NormalizeTableName with valid table name
        /// </summary>
        [Fact]
        public void NormalizeTableName_WithValidTableName_ShouldWrapInBackticks()
        {
            // Arrange
            var tableName = "Users";

            // Act
            var result = _adapter.NormalizeTableName(_mockConnection.Object, null, null, tableName);

            // Assert
            Assert.Equal("`Users`", result);
        }

        /// <summary>
        /// Test NormalizeTableName with schema
        /// </summary>
        [Fact]
        public void NormalizeTableName_WithSchema_ShouldIncludeSchemaWithBackticks()
        {
            // Arrange
            var schema = "mydb";
            var tableName = "Users";

            // Act
            var result = _adapter.NormalizeTableName(_mockConnection.Object, schema, null, tableName);

            // Assert
            Assert.Equal("`mydb`.`Users`", result);
        }

        /// <summary>
        /// Test NormalizeTableName with prefix
        /// </summary>
        [Fact]
        public void NormalizeTableName_WithPrefix_ShouldAddPrefixAndWrapInBackticks()
        {
            // Arrange
            var prefix = "App";
            var tableName = "Users";

            // Act
            var result = _adapter.NormalizeTableName(_mockConnection.Object, null, prefix, tableName);

            // Assert
            Assert.Equal("`AppUsers`", result);
        }

        /// <summary>
        /// Test NormalizeTableName with schema and prefix
        /// </summary>
        [Fact]
        public void NormalizeTableName_WithSchemaAndPrefix_ShouldIncludeBothWithBackticks()
        {
            // Arrange
            var schema = "mydb";
            var prefix = "App";
            var tableName = "Users";

            // Act
            var result = _adapter.NormalizeTableName(_mockConnection.Object, schema, prefix, tableName);

            // Assert
            Assert.Equal("`mydb`.`AppUsers`", result);
        }

        /// <summary>
        /// Test NormalizeTableName with table name that already has prefix
        /// </summary>
        [Fact]
        public void NormalizeTableName_WithExistingPrefix_ShouldNotDuplicatePrefix()
        {
            // Arrange
            var prefix = "App";
            var tableName = "AppUsers";

            // Act
            var result = _adapter.NormalizeTableName(_mockConnection.Object, null, prefix, tableName);

            // Assert
            Assert.Equal("`AppUsers`", result);
        }

        /// <summary>
        /// Test NormalizeTableName with null or empty table name
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NormalizeTableName_WithNullOrEmptyTableName_ShouldReturnOriginalValue(string tableName)
        {
            // Act
            var result = _adapter.NormalizeTableName(_mockConnection.Object, null, null, tableName);

            // Assert
            Assert.Equal(tableName, result);
        }

        /// <summary>
        /// Test NormalizeColumnName with valid column name
        /// </summary>
        [Fact]
        public void NormalizeColumnName_WithValidColumnName_ShouldWrapInBackticks()
        {
            // Arrange
            var columnName = "Name";

            // Act
            var result = _adapter.NormalizeColumnName(_mockConnection.Object, columnName);

            // Assert
            Assert.Equal("`Name`", result);
        }

        /// <summary>
        /// Test NormalizeColumnName with null or empty column name
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NormalizeColumnName_WithNullOrEmptyColumnName_ShouldReturnOriginalValue(string columnName)
        {
            // Act
            var result = _adapter.NormalizeColumnName(_mockConnection.Object, columnName);

            // Assert
            Assert.Equal(columnName, result);
        }

        /// <summary>
        /// Test NormalizeParameterName with valid parameter name
        /// </summary>
        [Fact]
        public void NormalizeParameterName_WithValidParameterName_ShouldPrefixWithAtSign()
        {
            // Arrange
            var parameterName = "userId";

            // Act
            var result = _adapter.NormalizeParameterName(_mockConnection.Object, parameterName);

            // Assert
            Assert.Equal("@userId", result);
        }

        /// <summary>
        /// Test NormalizeParameterName with parameter name that already has @ prefix
        /// </summary>
        [Fact]
        public void NormalizeParameterName_WithExistingAtPrefix_ShouldNotDuplicatePrefix()
        {
            // Arrange
            var parameterName = "@userId";

            // Act
            var result = _adapter.NormalizeParameterName(_mockConnection.Object, parameterName);

            // Assert
            Assert.Equal("@userId", result);
        }

        /// <summary>
        /// Test NormalizeParameterName with null or empty parameter name
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NormalizeParameterName_WithNullOrEmptyParameterName_ShouldReturnOriginalValue(string parameterName)
        {
            // Act
            var result = _adapter.NormalizeParameterName(_mockConnection.Object, parameterName);

            // Assert
            Assert.Equal(parameterName, result);
        }
    }
}
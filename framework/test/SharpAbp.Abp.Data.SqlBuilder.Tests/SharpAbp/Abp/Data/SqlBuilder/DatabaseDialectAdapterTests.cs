using Moq;
using Xunit;
using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for various database dialect adapters
    /// </summary>
    public class DatabaseDialectAdapterTests : AbpDataSqlBuilderTestBase
    {
        /// <summary>
        /// Test SqlServerDialectAdapter DatabaseProvider property
        /// </summary>
        [Fact]
        public void SqlServerDialectAdapter_DatabaseProvider_ReturnsSqlServer()
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.SqlServer) as SqlServerDialectAdapter;

            // Act
            var provider = adapter.DatabaseProvider;

            // Assert
            Assert.Equal(DatabaseProvider.SqlServer, provider);
        }

        /// <summary>
        /// Test SqlServerDialectAdapter NormalizeTableName method
        /// </summary>
        [Theory]
        [InlineData("Users", "[Users]")]
        [InlineData("dbo.Users", "[dbo].[Users]")]
        [InlineData("MySchema.MyTable", "[MySchema].[MyTable]")]
        [InlineData("", "")]
        public void SqlServerDialectAdapter_NormalizeTableName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.SqlServer) as SqlServerDialectAdapter;
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test SqlServerDialectAdapter NormalizeColumnName method
        /// </summary>
        [Theory]
        [InlineData("Id", "[Id]")]
        [InlineData("UserName", "[UserName]")]
        [InlineData("CreatedDate", "[CreatedDate]")]
        [InlineData("", "")]
        public void SqlServerDialectAdapter_NormalizeColumnName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.SqlServer);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeColumnName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test SqlServerDialectAdapter NormalizeParameterName method
        /// </summary>
        [Theory]
        [InlineData("id", "@id")]
        [InlineData("userName", "@userName")]
        [InlineData("createdDate", "@createdDate")]
        [InlineData("", "")]
        public void SqlServerDialectAdapter_NormalizeParameterName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.SqlServer);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeParameterName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test MySqlDialectAdapter DatabaseProvider property
        /// </summary>
        [Fact]
        public void MySqlDialectAdapter_DatabaseProvider_ReturnsMySql()
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.MySql) as MySqlDialectAdapter;

            // Act
            var provider = adapter.DatabaseProvider;

            // Assert
            Assert.Equal(DatabaseProvider.MySql, provider);
        }

        /// <summary>
        /// Test MySqlDialectAdapter NormalizeTableName method
        /// </summary>
        [Theory]
        [InlineData("users", "`users`")]
        [InlineData("mydb.users", "`mydb`.`users`")]
        [InlineData("my_table", "`my_table`")]
        [InlineData("", "")]
        public void MySqlDialectAdapter_NormalizeTableName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.MySql) as MySqlDialectAdapter;
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test MySqlDialectAdapter NormalizeColumnName method
        /// </summary>
        [Theory]
        [InlineData("id", "`id`")]
        [InlineData("user_name", "`user_name`")]
        [InlineData("created_date", "`created_date`")]
        [InlineData("", "")]
        public void MySqlDialectAdapter_NormalizeColumnName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.MySql);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeColumnName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test MySqlDialectAdapter NormalizeParameterName method
        /// </summary>
        [Theory]
        [InlineData("id", "@id")]
        [InlineData("userName", "@userName")]
        [InlineData("createdDate", "@createdDate")]
        [InlineData("", "")]
        public void MySqlDialectAdapter_NormalizeParameterName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.MySql);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeParameterName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test OracleDialectAdapter DatabaseProvider property
        /// </summary>
        [Fact]
        public void OracleDialectAdapter_DatabaseProvider_ReturnsOracle()
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.Oracle) as OracleDialectAdapter;

            // Act
            var provider = adapter.DatabaseProvider;

            // Assert
            Assert.Equal(DatabaseProvider.Oracle, provider);
        }

        /// <summary>
        /// Test OracleDialectAdapter NormalizeTableName method
        /// </summary>
        [Theory]
        [InlineData("USERS", "\"USERS\"")]
        [InlineData("HR.EMPLOYEES", "\"HR\".\"EMPLOYEES\"")]
        [InlineData("MY_TABLE", "\"MY_TABLE\"")]
        [InlineData("", "")]
        public void OracleDialectAdapter_NormalizeTableName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.Oracle) as OracleDialectAdapter;
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test OracleDialectAdapter NormalizeColumnName method
        /// </summary>
        [Theory]
        [InlineData("ID", "\"ID\"")]
        [InlineData("USER_NAME", "\"USER_NAME\"")]
        [InlineData("CREATED_DATE", "\"CREATED_DATE\"")]
        [InlineData("", "")]
        public void OracleDialectAdapter_NormalizeColumnName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.Oracle);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeColumnName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test OracleDialectAdapter NormalizeParameterName method
        /// </summary>
        [Theory]
        [InlineData("id", ":id")]
        [InlineData("userName", ":userName")]
        [InlineData("createdDate", ":createdDate")]
        [InlineData("", "")]
        public void OracleDialectAdapter_NormalizeParameterName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.Oracle);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeParameterName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test GaussDBDialectAdapter DatabaseProvider property
        /// </summary>
        [Fact]
        public void GaussDBDialectAdapter_DatabaseProvider_ReturnsGaussDB()
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.GaussDB) as GaussDBDialectAdapter;

            // Act
            var provider = adapter.DatabaseProvider;

            // Assert
            Assert.Equal(DatabaseProvider.GaussDB, provider);
        }

        /// <summary>
        /// Test GaussDBDialectAdapter NormalizeTableName method
        /// </summary>
        [Theory]
        [InlineData("users", "\"users\"")]
        [InlineData("public.users", "\"public\".\"users\"")]
        [InlineData("my_table", "\"my_table\"")]
        [InlineData("", "")]
        public void GaussDBDialectAdapter_NormalizeTableName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.GaussDB);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test GaussDBDialectAdapter NormalizeColumnName method
        /// </summary>
        [Theory]
        [InlineData("id", "\"id\"")]
        [InlineData("user_name", "\"user_name\"")]
        [InlineData("created_date", "\"created_date\"")]
        [InlineData("", "")]
        public void GaussDBDialectAdapter_NormalizeColumnName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.GaussDB);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeColumnName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test GaussDBDialectAdapter NormalizeParameterName method
        /// </summary>
        [Theory]
        [InlineData("id", "@id")]
        [InlineData("userName", "@userName")]
        [InlineData("createdDate", "@createdDate")]
        [InlineData("", "")]
        public void GaussDBDialectAdapter_NormalizeParameterName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.GaussDB);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeParameterName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test OpenGaussDialectAdapter DatabaseProvider property
        /// </summary>
        [Fact]
        public void OpenGaussDialectAdapter_DatabaseProvider_ReturnsOpenGauss()
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OpenGauss) as OpenGaussDialectAdapter;

            // Act
            var provider = adapter.DatabaseProvider;

            // Assert
            Assert.Equal(DatabaseProvider.OpenGauss, provider);
        }

        /// <summary>
        /// Test OpenGaussDialectAdapter NormalizeTableName method
        /// </summary>
        [Theory]
        [InlineData("users", "\"users\"")]
        [InlineData("public.users", "\"public\".\"users\"")]
        [InlineData("my_table", "\"my_table\"")]
        [InlineData("", "")]
        public void OpenGaussDialectAdapter_NormalizeTableName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OpenGauss);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test OpenGaussDialectAdapter NormalizeColumnName method
        /// </summary>
        [Theory]
        [InlineData("id", "\"id\"")]
        [InlineData("user_name", "\"user_name\"")]
        [InlineData("created_date", "\"created_date\"")]
        [InlineData("", "")]
        public void OpenGaussDialectAdapter_NormalizeColumnName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OpenGauss);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeColumnName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test OpenGaussDialectAdapter NormalizeParameterName method
        /// </summary>
        [Theory]
        [InlineData("id", "@id")]
        [InlineData("userName", "@userName")]
        [InlineData("createdDate", "@createdDate")]
        [InlineData("", "")]
        public void OpenGaussDialectAdapter_NormalizeParameterName_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OpenGauss);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeParameterName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test DmDialectAdapter DatabaseProvider property
        /// </summary>
        [Fact]
        public void DmDialectAdapter_DatabaseProvider_ReturnsDm()
        {
            // Arrange
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.Dm) as DmDialectAdapter;

            // Act
            var provider = adapter.DatabaseProvider;

            // Assert
            Assert.Equal(DatabaseProvider.Dm, provider);
        }

        /// <summary>
        /// Test DmDialectAdapter NormalizeTableName method with Oracle mode
        /// </summary>
        [Theory]
        [InlineData("USERS", "\"USERS\"")]
        [InlineData("HR.EMPLOYEES", "\"HR\".\"EMPLOYEES\"")]
        [InlineData("MY_TABLE", "\"MY_TABLE\"")]
        public void DmDialectAdapter_NormalizeTableName_OracleMode_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var mockDetector = new Mock<IDmDatabaseModeDetector>();
            mockDetector.Setup(x => x.DetectMode(It.IsAny<IDbConnection>()))
                       .Returns(DmDatabaseMode.Oracle);
            
            var mockLogger = new Mock<ILogger<DmDialectAdapter>>();
            var adapter = new DmDialectAdapter(mockLogger.Object, mockDetector.Object);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test DmDialectAdapter NormalizeTableName method with PostgreSQL mode
        /// </summary>
        [Theory]
        [InlineData("users", "\"users\"")]
        [InlineData("public.users", "\"public\".\"users\"")]
        [InlineData("my_table", "\"my_table\"")]
        public void DmDialectAdapter_NormalizeTableName_PostgreSqlMode_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var mockDetector = new Mock<IDmDatabaseModeDetector>();
            mockDetector.Setup(x => x.DetectMode(It.IsAny<IDbConnection>()))
                       .Returns(DmDatabaseMode.PostgreSql);
            
            var mockLogger = new Mock<ILogger<DmDialectAdapter>>();
            var adapter = new DmDialectAdapter(mockLogger.Object, mockDetector.Object);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test DmDialectAdapter NormalizeTableName method with MySQL mode
        /// </summary>
        [Theory]
        [InlineData("users", "`users`")]
        [InlineData("mydb.users", "`mydb`.`users`")]
        [InlineData("my_table", "`my_table`")]
        public void DmDialectAdapter_NormalizeTableName_MySqlMode_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var mockDetector = new Mock<IDmDatabaseModeDetector>();
            mockDetector.Setup(x => x.DetectMode(It.IsAny<IDbConnection>()))
                       .Returns(DmDatabaseMode.MySql);
            
            var mockLogger = new Mock<ILogger<DmDialectAdapter>>();
            var adapter = new DmDialectAdapter(mockLogger.Object, mockDetector.Object);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeTableName(mockConnection.Object, null, null, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test DmDialectAdapter NormalizeParameterName method with different modes
        /// </summary>
        [Theory]
        [InlineData(DmDatabaseMode.Oracle, "id", ":id")]
        [InlineData(DmDatabaseMode.PostgreSql, "id", "@id")]
        [InlineData(DmDatabaseMode.MySql, "id", "?id")]
        public void DmDialectAdapter_NormalizeParameterName_DifferentModes_ReturnsCorrectFormat(
            DmDatabaseMode mode, string input, string expected)
        {
            // Arrange
            var mockDetector = new Mock<IDmDatabaseModeDetector>();
            mockDetector.Setup(x => x.DetectMode(It.IsAny<IDbConnection>()))
                       .Returns(mode);
            
            var mockLogger = new Mock<ILogger<DmDialectAdapter>>();
            var adapter = new DmDialectAdapter(mockLogger.Object, mockDetector.Object);
            var mockConnection = new Mock<IDbConnection>();

            // Act
            var result = adapter.NormalizeParameterName(mockConnection.Object, input);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Test DmDialectAdapter with invalid database mode throws exception
        /// </summary>
        [Fact]
        public void DmDialectAdapter_InvalidDatabaseMode_ThrowsException()
        {
            // Arrange
            var mockDetector = new Mock<IDmDatabaseModeDetector>();
            mockDetector.Setup(x => x.DetectMode(It.IsAny<IDbConnection>()))
                       .Returns((DmDatabaseMode)999); // Invalid mode
            
            var mockLogger = new Mock<ILogger<DmDialectAdapter>>();
            var adapter = new DmDialectAdapter(mockLogger.Object, mockDetector.Object);
            var mockConnection = new Mock<IDbConnection>();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => adapter.NormalizeParameterName(mockConnection.Object, "test"));
        }
    }
}
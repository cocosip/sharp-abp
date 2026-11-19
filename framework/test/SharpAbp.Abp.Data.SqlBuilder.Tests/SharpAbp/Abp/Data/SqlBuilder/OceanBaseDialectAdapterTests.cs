using System;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for OceanBaseDialectAdapter
    /// OceanBase tenants run in Oracle or MySQL compatibility mode, selected by the mode detector
    /// </summary>
    public class OceanBaseDialectAdapterTests : AbpDataSqlBuilderTestBase
    {
        private readonly Mock<IDbConnection> _mockConnection = new();

        [Fact]
        public void Adapter_ShouldBeRegisteredForKeyedProvider()
        {
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OceanBase);

            Assert.IsType<OceanBaseDialectAdapter>(adapter);
            Assert.Equal(DatabaseProvider.OceanBase, adapter!.DatabaseProvider);
        }

        [Fact]
        public void Normalize_WithDefaultModeDetector_UsesOracleMode()
        {
            var adapter = ServiceProvider.GetKeyedService<IDatabaseDialectAdapter>(DatabaseProvider.OceanBase)!;

            Assert.Equal("\"Users\"", adapter.NormalizeTableName(_mockConnection.Object, null, null, "Users"));
            Assert.Equal("\"Name\"", adapter.NormalizeColumnName(_mockConnection.Object, "Name"));
            Assert.Equal(":id", adapter.NormalizeParameterName(_mockConnection.Object, "id"));
        }

        [Theory]
        [InlineData("Users", null, null, "\"Users\"")]
        [InlineData("Users", "HR", null, "\"HR\".\"Users\"")]
        [InlineData("Users", null, "App", "\"AppUsers\"")]
        [InlineData("Users", "HR", "App", "\"HR\".\"AppUsers\"")]
        [InlineData("HR.Users", null, null, "\"HR\".\"Users\"")]
        public void OracleMode_NormalizeTableName_ReturnsDoubleQuotedName(
            string tableName, string schema, string prefix, string expected)
        {
            var adapter = CreateAdapter(OceanBaseDatabaseMode.Oracle);

            var result = adapter.NormalizeTableName(_mockConnection.Object, schema, prefix, tableName);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Users", null, null, "`Users`")]
        [InlineData("Users", "mydb", null, "`mydb`.`Users`")]
        [InlineData("Users", null, "App", "`AppUsers`")]
        [InlineData("Users", "mydb", "App", "`mydb`.`AppUsers`")]
        [InlineData("mydb.Users", null, null, "`mydb`.`Users`")]
        public void MySqlMode_NormalizeTableName_ReturnsBacktickQuotedName(
            string tableName, string schema, string prefix, string expected)
        {
            var adapter = CreateAdapter(OceanBaseDatabaseMode.MySql);

            var result = adapter.NormalizeTableName(_mockConnection.Object, schema, prefix, tableName);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void OracleMode_NormalizeParameterName_ShouldNotDuplicateColonPrefix()
        {
            var adapter = CreateAdapter(OceanBaseDatabaseMode.Oracle);

            Assert.Equal(":id", adapter.NormalizeParameterName(_mockConnection.Object, ":id"));
        }

        [Fact]
        public void MySqlMode_NormalizeParameterName_ShouldReturnPositionalMarker()
        {
            var adapter = CreateAdapter(OceanBaseDatabaseMode.MySql);

            Assert.Equal("?", adapter.NormalizeParameterName(_mockConnection.Object, "id"));
        }

        [Fact]
        public void MySqlMode_NormalizeColumnName_ReturnsBacktickQuotedName()
        {
            var adapter = CreateAdapter(OceanBaseDatabaseMode.MySql);

            Assert.Equal("`Name`", adapter.NormalizeColumnName(_mockConnection.Object, "Name"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NormalizeMethods_WithNullOrEmptyValue_ShouldReturnOriginalValue(string value)
        {
            var adapter = CreateAdapter(OceanBaseDatabaseMode.Oracle);

            Assert.Equal(value, adapter.NormalizeTableName(_mockConnection.Object, null, null, value));
            Assert.Equal(value, adapter.NormalizeColumnName(_mockConnection.Object, value));
            Assert.Equal(value, adapter.NormalizeParameterName(_mockConnection.Object, value));
        }

        [Fact]
        public void UnsupportedMode_ShouldThrow()
        {
            var adapter = CreateAdapter((OceanBaseDatabaseMode)999);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => adapter.NormalizeParameterName(_mockConnection.Object, "id"));
        }

        private static OceanBaseDialectAdapter CreateAdapter(OceanBaseDatabaseMode mode)
        {
            var modeDetector = new Mock<IOceanBaseDatabaseModeDetector>();
            modeDetector.Setup(x => x.DetectMode(It.IsAny<IDbConnection>())).Returns(mode);
            return new OceanBaseDialectAdapter(modeDetector.Object);
        }
    }
}

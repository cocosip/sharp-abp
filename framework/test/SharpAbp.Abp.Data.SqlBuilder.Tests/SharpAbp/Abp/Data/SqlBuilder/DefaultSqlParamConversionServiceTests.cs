#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for DefaultSqlParamConversionService
    /// </summary>
    public class DefaultSqlParamConversionServiceTests : AbpDataSqlBuilderTestBase
    {
        private readonly ISqlParamConversionService _service;

        public DefaultSqlParamConversionServiceTests()
        {
            _service = GetRequiredService<ISqlParamConversionService>();
        }

        /// <summary>
        /// Test that ConvertParameterAsync works with string entity using StringEntitySqlParamConversionContributor
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_StringEntity_ReturnsConvertedData()
        {
            // Arrange
            var provider = DatabaseProvider.SqlServer;
            var entityType = typeof(string);
            var inputData = "test string";

            // Act
            var result = await _service.ConvertParameterAsync(provider, entityType, inputData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Converted: test string", result);
        }

        /// <summary>
        /// Test that ConvertParameterAsync works with integer entity using IntegerEntitySqlParamConversionContributor
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_IntegerEntity_ReturnsConvertedData()
        {
            // Arrange
            var provider = DatabaseProvider.MySql;
            var entityType = typeof(int);
            var inputData = 42;

            // Act
            var result = await _service.ConvertParameterAsync(provider, entityType, inputData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(84, result); // 42 * 2
        }

        /// <summary>
        /// Test that ConvertParameterAsync works with DateTime entity using DateTimeEntitySqlParamConversionContributor
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_DateTimeEntity_ReturnsConvertedData()
        {
            // Arrange
            var provider = DatabaseProvider.Oracle;
            var entityType = typeof(DateTime);
            var inputData = new DateTime(2023, 1, 1, 12, 0, 0);

            // Act
            var result = await _service.ConvertParameterAsync(provider, entityType, inputData);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is TestDateTimeOutput);

            var r = result as TestDateTimeOutput;

            Assert.Equal("2023-01-01 12:00:00", r?.Data);
        }

        /// <summary>
        /// Test that ConvertParameterAsync returns null when input data is null
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_NullInputData_ReturnsNull()
        {
            // Arrange
            var provider = DatabaseProvider.SqlServer;
            var entityType = typeof(TestEntity);
            object? inputData = null;

            // Act
            var result = await _service.ConvertParameterAsync(provider, entityType, inputData);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Test that ConvertParameterAsync works with generic object using GenericObjectSqlParamConversionContributor
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_GenericObject_ReturnsConvertedData()
        {
            // Arrange
            var provider = DatabaseProvider.SqlServer;
            var entityType = typeof(TestEntity);
            var inputData = new TestEntity { Id = 1, Name = "Test" };

            // Act
            var result = await _service.ConvertParameterAsync(provider, entityType, inputData);

            // Assert
            Assert.NotNull(result);

        }

        /// <summary>
        /// Test that ConvertParameterAsync returns original data for unsupported types
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_UnsupportedType_ReturnsOriginalData()
        {
            // Arrange
            var provider = DatabaseProvider.SqlServer;
            var entityType = typeof(decimal); // Not handled by any of our test contributors
            var inputData = 123.45m;

            // Act
            var result = await _service.ConvertParameterAsync(provider, entityType, inputData);

            // Assert
            Assert.Equal(inputData, result);
        }

        /// <summary>
        /// Test that ConvertParameterAsync works with cancellation token
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_WithCancellationToken_WorksCorrectly()
        {
            // Arrange
            var provider = DatabaseProvider.SqlServer;
            var entityType = typeof(string);
            var inputData = "test";
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _service.ConvertParameterAsync(provider, entityType, inputData, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Converted: test", result);
        }

        /// <summary>
        /// Test entity class for testing purposes
        /// </summary>
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
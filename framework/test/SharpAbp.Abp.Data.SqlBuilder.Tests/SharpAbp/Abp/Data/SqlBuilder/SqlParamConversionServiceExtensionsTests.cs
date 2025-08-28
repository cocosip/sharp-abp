#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Unit tests for SqlParamConversionServiceExtensions
    /// </summary>
    public class SqlParamConversionServiceExtensionsTests : AbpDataSqlBuilderTestBase
    {
        private readonly ISqlParamConversionService _service;

        public SqlParamConversionServiceExtensionsTests()
        {
            _service = GetRequiredService<ISqlParamConversionService>();
        }

        /// <summary>
        /// Test that generic ConvertParameterAsync calls the base method with correct parameters
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_Generic_CallsBaseMethodWithCorrectParameters()
        {
            // Arrange
            var provider = DatabaseProvider.SqlServer;
            var inputData = new TestEntity { Id = 1, Name = "Test" };
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _service.ConvertParameterAsync(provider, inputData, cancellationToken);

            // Assert
            // Since no contributors are registered, the result should be the original input data
            Assert.Equal(inputData, result);
        }

        /// <summary>
        /// Test that generic ConvertParameterAsync works with null input data
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_Generic_WithNullInputData_CallsBaseMethod()
        {
            // Arrange
            var provider = DatabaseProvider.MySql;
            TestEntity? inputData = null;
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _service.ConvertParameterAsync(provider, inputData, cancellationToken);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Test that generic ConvertParameterAsync works with different entity types
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_Generic_WithDifferentEntityTypes_CallsBaseMethodWithCorrectType()
        {
            // Arrange
            var provider = DatabaseProvider.Oracle;
            var stringData = "test string";
            var intData = 42;
            var cancellationToken = new CancellationToken();

            // Act
            var stringResult = await _service.ConvertParameterAsync(provider, stringData, cancellationToken);
            var intResult = await _service.ConvertParameterAsync(provider, intData, cancellationToken);

            // Assert
            // Since no contributors are registered, the results should be the original input data
            Assert.Equal(stringData, stringResult);
            Assert.Equal(intData, intResult);
        }

        /// <summary>
        /// Test that generic ConvertParameterAsync uses default cancellation token when not provided
        /// </summary>
        [Fact]
        public async Task ConvertParameterAsync_Generic_WithoutCancellationToken_UsesDefault()
        {
            // Arrange
            var provider = DatabaseProvider.SqlServer;
            var inputData = new TestEntity { Id = 1, Name = "Test" };

            // Act
            var result = await _service.ConvertParameterAsync(provider, inputData);

            // Assert
            // Since no contributors are registered, the result should be the original input data
            Assert.Equal(inputData, result);
        }

 
        /// <summary>
        /// Test entity class for testing purposes
        /// </summary>
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// Converted test entity class for testing purposes
        /// </summary>
        private class ConvertedTestEntity
        {
            public int ConvertedId { get; set; }
            public string ConvertedName { get; set; } = string.Empty;
        }

        /// <summary>
        /// Test SQL parameter conversion contributor for testing purposes
        /// </summary>
        private class TestSqlParamConversionContributor : ISqlParamConversionContributor, ITransientDependency
        {
            /// <summary>
            /// Check if this contributor can handle the given database provider and entity type
            /// </summary>
            /// <param name="context">The parameter binding context containing database provider and entity type information</param>
            /// <param name="cancellationToken">Cancellation token</param>
            /// <returns>True if this contributor can handle the conversion, false otherwise</returns>
            public Task<bool> IsMatchAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(context.EntityType == typeof(TestEntity));
            }

            /// <summary>
            /// Process the parameter conversion for TestEntity
            /// </summary>
            /// <param name="context">The parameter binding context containing database provider, entity type and input data</param>
            /// <param name="cancellationToken">Cancellation token</param>
            /// <returns>The converted parameter data</returns>
            public Task<object> ProcessAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
            {
                if (context.InputData is TestEntity testEntity)
                {
                    var converted = new ConvertedTestEntity
                    {
                        ConvertedId = testEntity.Id,
                        ConvertedName = $"Converted_{testEntity.Name}"
                    };
                    return Task.FromResult<object>(converted);
                }
                return Task.FromResult<object>(context.InputData ?? new object());
            }
        }
    }
}
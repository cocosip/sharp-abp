using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Test SQL parameter conversion contributor for string entities
    /// </summary>
    public class StringEntitySqlParamConversionContributor : ISqlParamConversionContributor, ITransientDependency
    {
        /// <summary>
        /// Check if this contributor can handle string entity types
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider and entity type information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if this contributor can handle the conversion, false otherwise</returns>
        public Task<bool> IsMatchAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(context.EntityType == typeof(string) && context.DatabaseProvider == DatabaseProvider.SqlServer);
        }

        /// <summary>
        /// Process the parameter conversion for string entities
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider, entity type and input data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The converted parameter data</returns>
        public Task<object> ProcessAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            if (context.InputData is string stringData)
            {
                var converted = $"Converted: {stringData}";
                return Task.FromResult<object>(converted);
            }
            return Task.FromResult<object>(context.InputData ?? string.Empty);
        }
    }

    /// <summary>
    /// Test SQL parameter conversion contributor for integer entities
    /// </summary>
    public class IntegerEntitySqlParamConversionContributor : ISqlParamConversionContributor, ITransientDependency
    {
        /// <summary>
        /// Check if this contributor can handle integer entity types
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider and entity type information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if this contributor can handle the conversion, false otherwise</returns>
        public Task<bool> IsMatchAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(context.EntityType == typeof(int) && context.DatabaseProvider == DatabaseProvider.MySql);
        }

        /// <summary>
        /// Process the parameter conversion for integer entities
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider, entity type and input data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The converted parameter data</returns>
        public Task<object> ProcessAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            if (context.InputData is int intData)
            {
                // Convert by multiplying by 2
                var converted = intData * 2;
                return Task.FromResult<object>(converted);
            }
            return Task.FromResult<object>(context.InputData ?? 0);
        }
    }

    /// <summary>
    /// Test SQL parameter conversion contributor for DateTime entities
    /// </summary>
    public class DateTimeEntitySqlParamConversionContributor : ISqlParamConversionContributor, ITransientDependency
    {
        /// <summary>
        /// Check if this contributor can handle DateTime entity types
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider and entity type information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if this contributor can handle the conversion, false otherwise</returns>
        public Task<bool> IsMatchAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(context.EntityType == typeof(DateTime) && context.DatabaseProvider == DatabaseProvider.Oracle);
        }

        /// <summary>
        /// Process the parameter conversion for DateTime entities
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider, entity type and input data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The converted parameter data</returns>
        public Task<object> ProcessAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            if (context.InputData is DateTime dateTimeData)
            {
                // Convert to Oracle-specific format
                var converted = new TestDateTimeOutput
                {
                    Data = dateTimeData.ToString("yyyy-MM-dd HH:mm:ss"),
                    Timestamp = dateTimeData.Ticks
                };
                return Task.FromResult<object>(converted);
            }
            return Task.FromResult<object>(context.InputData ?? DateTime.MinValue);
        }
    }

    public class TestDateTimeOutput
    {
        public string Data { get; set; }
        public long Timestamp { get; set; }
    }

    /// <summary>
    /// Test SQL parameter conversion contributor for generic object entities
    /// </summary>
    public class GenericObjectSqlParamConversionContributor : ISqlParamConversionContributor, ITransientDependency
    {
        /// <summary>
        /// Check if this contributor can handle generic object entity types
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider and entity type information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if this contributor can handle the conversion, false otherwise</returns>
        public Task<bool> IsMatchAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            // This contributor handles any object type for PostgreSQL
            return Task.FromResult(context.DatabaseProvider == DatabaseProvider.PostgreSql);
        }

        /// <summary>
        /// Process the parameter conversion for generic object entities
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider, entity type and input data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The converted parameter data</returns>
        public Task<object> ProcessAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            if (context.InputData is TestEntity testEntity)
            {
                // Convert to PostgreSQL-specific format
                var converted = new { Id = testEntity.Id, Name = testEntity.Name, PostgreSqlConverted = true };
                return Task.FromResult<object>(converted);
            }
            return Task.FromResult<object>(context.InputData ?? new object());
        }
    }

    /// <summary>
    /// Test SQL parameter conversion contributor that always returns null (for testing null handling)
    /// </summary>
    public class NullReturningSqlParamConversionContributor : ISqlParamConversionContributor, ITransientDependency
    {
        /// <summary>
        /// Check if this contributor can handle the conversion (always matches for testing)
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider and entity type information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if this contributor can handle the conversion, false otherwise</returns>
        public Task<bool> IsMatchAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            // Only match for specific test scenarios
            return Task.FromResult(context.EntityType == typeof(object) && context.DatabaseProvider == DatabaseProvider.Sqlite);
        }

        /// <summary>
        /// Process the parameter conversion (always returns null for testing)
        /// </summary>
        /// <param name="context">The parameter binding context containing database provider, entity type and input data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Always returns null for testing null handling</returns>
        public Task<object> ProcessAsync(ParameterBindingContext context, CancellationToken cancellationToken = default)
        {
            // Always return null to test null handling in the service
            return Task.FromResult<object>(null!);
        }
    }

    /// <summary>
    /// Test entity class for testing purposes
    /// </summary>
    public class TestEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the entity name
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Converted test entity class for testing purposes
    /// </summary>
    public class ConvertedTestEntity
    {
        /// <summary>
        /// Gets or sets the converted entity identifier
        /// </summary>
        public int ConvertedId { get; set; }

        /// <summary>
        /// Gets or sets the converted entity name
        /// </summary>
        public string ConvertedName { get; set; } = string.Empty;
    }
}
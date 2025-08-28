using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Service for converting SQL parameters based on database provider and entity type
    /// </summary>
    public interface ISqlParamConversionService
    {
        /// <summary>
        /// Converts the input parameter to the appropriate format for the specified database provider
        /// </summary>
        /// <param name="provider">The target database provider</param>
        /// <param name="entityType">The entity type being processed</param>
        /// <param name="inputData">The input data to be converted</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The converted parameter object</returns>
        Task<object?> ConvertParameterAsync(DatabaseProvider provider, Type entityType, object? inputData, CancellationToken cancellationToken = default);
    }
}
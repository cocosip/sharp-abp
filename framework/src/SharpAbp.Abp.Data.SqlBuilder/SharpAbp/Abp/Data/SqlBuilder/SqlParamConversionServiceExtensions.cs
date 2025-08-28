using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Extension methods for ISqlParamConversionService
    /// </summary>
    public static class SqlParamConversionServiceExtensions
    {
        /// <summary>
        /// Converts the input parameter to the appropriate format for the specified database provider with generic type support
        /// </summary>
        /// <typeparam name="T">The entity type being processed</typeparam>
        /// <param name="service">The SQL parameter conversion service</param>
        /// <param name="provider">The target database provider</param>
        /// <param name="inputData">The input data to be converted</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The converted parameter object</returns>
        public static async Task<object?> ConvertParameterAsync<T>(
            this ISqlParamConversionService service,
            DatabaseProvider provider,
            T inputData,
            CancellationToken cancellationToken = default)
        {
            return await service.ConvertParameterAsync(provider, typeof(T), inputData, cancellationToken);
        }
    }
}
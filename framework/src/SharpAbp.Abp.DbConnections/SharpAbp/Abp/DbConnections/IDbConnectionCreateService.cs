using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionCreateService
    {
        /// <summary>
        /// Creates a database connection for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection to create</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when no database connection information is found for the specified name or when the database provider is not supported</exception>
        [NotNull]
        Task<IDbConnection?> CreateAsync([NotNull] string dbConnectionName);

        /// <summary>
        /// Creates a database connection using the specified connection information
        /// </summary>
        /// <param name="dbConnectionInfo">The database connection information</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when the database provider is not supported</exception>
        [NotNull]
        Task<IDbConnection?> CreateAsync([NotNull] DbConnectionInfo dbConnectionInfo);
    }
}

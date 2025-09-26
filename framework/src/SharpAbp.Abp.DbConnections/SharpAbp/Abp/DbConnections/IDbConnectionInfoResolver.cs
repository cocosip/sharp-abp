using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Defines an interface for resolving database connection information by name
    /// </summary>
    public interface IDbConnectionInfoResolver
    {
        /// <summary>
        /// Resolves database connection information for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection to resolve</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information.</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when no database connection configuration is found for the specified name</exception>
        Task<DbConnectionInfo> ResolveAsync([NotNull] string dbConnectionName);
    }
}
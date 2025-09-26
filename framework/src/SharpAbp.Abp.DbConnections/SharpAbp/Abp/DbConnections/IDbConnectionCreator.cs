using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Defines an interface for creating database connections
    /// </summary>
    public interface IDbConnectionCreator
    {
        /// <summary>
        /// Determines whether this creator can handle the specified database connection
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection</param>
        /// <param name="dbConnectionInfo">The database connection information</param>
        /// <returns>True if this creator can handle the specified database connection; otherwise, false</returns>
        bool IsMatch(string dbConnectionName, DbConnectionInfo dbConnectionInfo);

        /// <summary>
        /// Creates a database connection asynchronously
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection</param>
        /// <param name="dbConnectionInfo">The database connection information</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created database connection.</returns>
        Task<IDbConnection> CreateAsync(string dbConnectionName, DbConnectionInfo dbConnectionInfo);
    }
}
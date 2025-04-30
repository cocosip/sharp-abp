using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionInfoManager : IDomainService
    {
        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="connectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        Task<DatabaseConnectionInfo> CreateAsync(string name, string databaseProvider, string connectionString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="databaseConnectionInfo"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="connectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfo> UpdateAsync(DatabaseConnectionInfo databaseConnectionInfo, string databaseProvider, string connectionString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Change name
        /// </summary>
        /// <param name="databaseConnectionInfo"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        Task<DatabaseConnectionInfo> ChangeNameAsync(DatabaseConnectionInfo databaseConnectionInfo, string name, CancellationToken cancellationToken = default);
    }
}

using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionInfoManager : IDomainService
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="databaseConnectionInfo"></param>
        /// <returns></returns>
        Task CreateAsync(DatabaseConnectionInfo databaseConnectionInfo);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, string name, string databaseProvider, string connectionString);
    }
}

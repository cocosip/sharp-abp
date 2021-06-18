using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionInfoRepository : IBasicRepository<DatabaseConnectionInfo, Guid>
    {
        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfo> FindByNameAsync([NotNull] string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfo> FindExpectedByNameAsync(string name, Guid? expectedId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<DatabaseConnectionInfo>> GetListAsync(int skipCount, int maxResultCount, string sorting = null, string name = "", string databaseProvider = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string name = "", string databaseProvider = "", CancellationToken cancellationToken = default);
    }
}

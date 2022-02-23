using Microsoft.Extensions.Localization;
using SharpAbp.Abp.DbConnectionsManagement.Localization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoManager : DomainService
    {
        protected IStringLocalizer<DbConnectionsManagementResource> Localizer { get; }
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }
        public DatabaseConnectionInfoManager(
            IStringLocalizer<DbConnectionsManagementResource> localizer,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            Localizer = localizer;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="databaseConnectionInfo"></param>
        /// <returns></returns>
        public virtual async Task<DatabaseConnectionInfo> CreateAsync(DatabaseConnectionInfo databaseConnectionInfo)
        {
            var queryDatabaseConnectionInfo = await ConnectionInfoRepository.FindExpectedByNameAsync(databaseConnectionInfo.Name);
            if (queryDatabaseConnectionInfo != null)
            {
                throw new UserFriendlyException(Localizer["DbConnectionsManagement.DuplicateName", databaseConnectionInfo.Name]);
            }

            return await ConnectionInfoRepository.InsertAsync(databaseConnectionInfo);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual async Task<DatabaseConnectionInfo> UpdateAsync(
            Guid id,
            string name,
            string databaseProvider,
            string connectionString)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.GetAsync(id);

            databaseConnectionInfo.Update(name, databaseProvider, connectionString);
            return await ConnectionInfoRepository.UpdateAsync(databaseConnectionInfo);
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.DbConnectionsManagement.Localization;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoManager : DomainService, IDatabaseConnectionInfoManager
    {
        protected IStringLocalizer<DbConnectionsManagementResource> Localizer { get; }
        protected IDatabaseConnectionInfoRepository DatabaseConnectionInfoRepository { get; }
        public DatabaseConnectionInfoManager(
            IStringLocalizer<DbConnectionsManagementResource> localizer,
            IDatabaseConnectionInfoRepository databaseConnectionInfoRepository)
        {
            Localizer = localizer;
            DatabaseConnectionInfoRepository = databaseConnectionInfoRepository;
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="databaseConnectionInfo"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(DatabaseConnectionInfo databaseConnectionInfo)
        {
            var queryDatabaseConnectionInfo = await DatabaseConnectionInfoRepository.FindExpectedByNameAsync(databaseConnectionInfo.Name);
            if (queryDatabaseConnectionInfo != null)
            {
                throw new UserFriendlyException(Localizer["DbConnectionsManagement.DuplicateName", databaseConnectionInfo.Name]);
            }

            await DatabaseConnectionInfoRepository.InsertAsync(databaseConnectionInfo);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(Guid id, string name, string databaseProvider, string connectionString)
        {
            var databaseConnectionInfo = await DatabaseConnectionInfoRepository.GetAsync(id);

            var queryDatabaseConnectionInfo = await DatabaseConnectionInfoRepository.FindExpectedByNameAsync(name, id);
            if (queryDatabaseConnectionInfo != null)
            {
                throw new UserFriendlyException(Localizer["DbConnectionsManagement.DuplicateName", databaseConnectionInfo.Name]);
            }

            databaseConnectionInfo.Update(name, databaseProvider, connectionString);

            await DatabaseConnectionInfoRepository.UpdateAsync(databaseConnectionInfo);
        }
    }
}

using System.Threading;
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
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="connectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public virtual async Task<DatabaseConnectionInfo> CreateAsync(string name, string databaseProvider, string connectionString, CancellationToken cancellationToken = default)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.FindExpectedByNameAsync(name, cancellationToken: cancellationToken);
            if (databaseConnectionInfo != null)
            {
                throw new UserFriendlyException(Localizer["DbConnectionsManagement.DuplicateName", name]);
            }
            databaseConnectionInfo = new DatabaseConnectionInfo(GuidGenerator.Create(), name, databaseProvider, connectionString);
            return databaseConnectionInfo;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="databaseConnectionInfo"></param>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="connectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<DatabaseConnectionInfo> UpdateAsync(
            DatabaseConnectionInfo databaseConnectionInfo,
            string name,
            string databaseProvider,
            string connectionString,
            CancellationToken cancellationToken = default)
        {
            databaseConnectionInfo.Update(name, databaseProvider, connectionString);
            return await ConnectionInfoRepository.UpdateAsync(databaseConnectionInfo, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Change name
        /// </summary>
        /// <param name="databaseConnectionInfo"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public virtual async Task<DatabaseConnectionInfo> ChangeNameAsync(
            DatabaseConnectionInfo databaseConnectionInfo,
            string name,
            CancellationToken cancellationToken = default)
        {
            var queryDatabaseConnectionInfo = await ConnectionInfoRepository.FindExpectedByNameAsync(databaseConnectionInfo.Name, databaseConnectionInfo.Id, cancellationToken: cancellationToken);
            if (queryDatabaseConnectionInfo != null)
            {
                throw new UserFriendlyException(Localizer["DbConnectionsManagement.DuplicateName", databaseConnectionInfo.Name]);
            }

            databaseConnectionInfo.ChangeName(name);
            return await ConnectionInfoRepository.UpdateAsync(databaseConnectionInfo, cancellationToken: cancellationToken);
        }
    }
}

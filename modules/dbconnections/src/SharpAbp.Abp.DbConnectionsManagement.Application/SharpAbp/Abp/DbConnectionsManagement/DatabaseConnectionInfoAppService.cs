using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
    public class DatabaseConnectionInfoAppService : DbConnectionsManagementAppServiceBase, IDatabaseConnectionInfoAppService
    {
        protected IDatabaseConnectionInfoManager DatabaseConnectionInfoManager { get; }
        protected IDatabaseConnectionInfoRepository DatabaseConnectionInfoRepository { get; }
        public DatabaseConnectionInfoAppService(
            IDatabaseConnectionInfoManager databaseConnectionInfoManager,
            IDatabaseConnectionInfoRepository databaseConnectionInfoRepository)
        {
            DatabaseConnectionInfoManager = databaseConnectionInfoManager;
            DatabaseConnectionInfoRepository = databaseConnectionInfoRepository;
        }

        /// <summary>
        /// Get DatabaseConnectionInfo by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
        public virtual async Task<DatabaseConnectionInfoDto> GetAsync(Guid id)
        {
            var databaseConnectionInfo = await DatabaseConnectionInfoRepository.GetAsync(id);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(databaseConnectionInfo);
        }

        /// <summary>
        /// Find DatabaseConnectionInfo by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
        public virtual async Task<DatabaseConnectionInfoDto> FindByNameAsync([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var databaseConnectionInfo = await DatabaseConnectionInfoRepository.FindByNameAsync(name);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(databaseConnectionInfo);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
        public virtual async Task<PagedResultDto<DatabaseConnectionInfoDto>> GetPagedListAsync(
            DatabaseConnectionInfoPagedRequestDto input)
        {
            var count = await DatabaseConnectionInfoRepository.GetCountAsync(input.Name, input.DatabaseProvider);

            var databaseConnectionInfos = await DatabaseConnectionInfoRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Name,
                input.DatabaseProvider);

            return new PagedResultDto<DatabaseConnectionInfoDto>(
              count,
              ObjectMapper.Map<List<DatabaseConnectionInfo>, List<DatabaseConnectionInfoDto>>(databaseConnectionInfos)
              );
        }


        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Create)]
        public virtual async Task<Guid> CreateAsync(CreateDatabaseConnectionInfoDto input)
        {
            return default;
        }
    }
}

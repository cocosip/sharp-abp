using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
    public class DatabaseConnectionInfoAppService : DbConnectionsManagementAppServiceBase, IDatabaseConnectionInfoAppService
    {
        protected DatabaseConnectionInfoManager ConnectionInfoManager { get; }
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }
        public DatabaseConnectionInfoAppService(
            DatabaseConnectionInfoManager connectionInfoManager,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            ConnectionInfoManager = connectionInfoManager;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Get DatabaseConnectionInfo by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Default)]
        public virtual async Task<DatabaseConnectionInfoDto> GetAsync(Guid id)
        {
            var databaseConnectionInfo = await ConnectionInfoRepository.GetAsync(id);
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

            var databaseConnectionInfo = await ConnectionInfoRepository.FindByNameAsync(name);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(databaseConnectionInfo);
        }

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <returns></returns>
        public virtual async Task<List<DatabaseConnectionInfoDto>> GetListAsync(
            string sorting = null,
            string name = "",
            string databaseProvider = "")
        {
            var databaseConnectionInfos = await ConnectionInfoRepository.GetListAsync(
                sorting,
                name,
                databaseProvider);
            return ObjectMapper.Map<List<DatabaseConnectionInfo>, List<DatabaseConnectionInfoDto>>(databaseConnectionInfos);
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
            var count = await ConnectionInfoRepository.GetCountAsync(input.Name, input.DatabaseProvider);

            var databaseConnectionInfos = await ConnectionInfoRepository.GetPagedListAsync(
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

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Create)]
        public virtual async Task<DatabaseConnectionInfoDto> CreateAsync(CreateDatabaseConnectionInfoDto input)
        {
            var databaseConnectionInfo = new DatabaseConnectionInfo(
                GuidGenerator.Create(),
                input.Name,
                input.DatabaseProvider,
                input.ConnectionString);

            await ConnectionInfoManager.CreateAsync(databaseConnectionInfo);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(databaseConnectionInfo);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Update)]
        public virtual async Task<DatabaseConnectionInfoDto> UpdateAsync(Guid id, UpdateDatabaseConnectionInfoDto input)
        {
            var databaseConnectionInfo = await ConnectionInfoManager.UpdateAsync(
                id,
                input.DatabaseProvider,
                input.ConnectionString);
            return ObjectMapper.Map<DatabaseConnectionInfo, DatabaseConnectionInfoDto>(databaseConnectionInfo);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(DbConnectionsManagementPermissions.DatabaseConnectionInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await ConnectionInfoRepository.DeleteAsync(id);
        }
    }
}

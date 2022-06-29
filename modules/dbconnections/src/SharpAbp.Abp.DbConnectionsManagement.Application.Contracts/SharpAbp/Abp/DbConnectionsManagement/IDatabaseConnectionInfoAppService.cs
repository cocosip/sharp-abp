using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionInfoAppService : IApplicationService
    {
        /// <summary>
        /// Get DatabaseConnectionInfo by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfoDto> GetAsync(Guid id);

        /// <summary>
        /// Find DatabaseConnectionInfo by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfoDto> FindByNameAsync(string name);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="databaseProvider"></param>
        /// <returns></returns>
        Task<List<DatabaseConnectionInfoDto>> GetListAsync(string sorting = null, string name = "", string databaseProvider = "");

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<DatabaseConnectionInfoDto>> GetPagedListAsync(
          DatabaseConnectionInfoPagedRequestDto input);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfoDto> CreateAsync(CreateDatabaseConnectionInfoDto input);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfoDto> UpdateAsync(Guid id, UpdateDatabaseConnectionInfoDto input);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}

using JetBrains.Annotations;
using System;
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
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<DatabaseConnectionInfoDto>> GetPagedListAsync(
          DatabaseConnectionInfoPagedRequestDto input);
    }
}

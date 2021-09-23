using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.AuditLogging
{
    public interface IAuditLogAppService : IApplicationService
    {

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AuditLogDto> GetAsync(Guid id);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<AuditLogDto>> GetPagedListAsync(
           AuditLogPagedRequestDto input);
    

    }
}

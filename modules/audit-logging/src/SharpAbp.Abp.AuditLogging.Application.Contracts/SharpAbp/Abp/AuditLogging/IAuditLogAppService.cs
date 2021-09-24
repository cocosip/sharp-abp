using System;
using System.Collections.Generic;
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

        /// <summary>
        /// EntityChange paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<EntityChangeDto>> GetEntityChangePagedListAsync(EntityChangePagedRequestDto input);

        /// <summary>
        /// Get entityChange with username
        /// </summary>
        /// <param name="entityChangeId"></param>
        /// <returns></returns>
        Task<EntityChangeWithUsernameDto> GetEntityChangeWithUsernameAsync(Guid entityChangeId);

        /// <summary>
        /// Get entityChanges with username
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityTypeFullName"></param>
        /// <returns></returns>
        Task<List<EntityChangeWithUsernameDto>> GetEntityChangesWithUsernameAsync(string entityId, string entityTypeFullName);
    }
}

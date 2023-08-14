using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AuditLogging;

namespace SharpAbp.Abp.AuditLogging
{
    [Authorize(AuditLoggingPermissions.AuditLogs.Default)]
    public class AuditLogAppService : AuditLoggingAppServiceBase, IAuditLogAppService
    {
        protected IAuditLogRepository AuditLogRepository { get; }
        public AuditLogAppService(IAuditLogRepository auditLogRepository)
        {
            AuditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<AuditLogDto> GetAsync(Guid id)
        {
            var auditLog = await AuditLogRepository.GetAsync(id);
            return ObjectMapper.Map<AuditLog, AuditLogDto>(auditLog);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<AuditLogDto>> GetPagedListAsync(
           AuditLogPagedRequestDto input)
        {
            var count = await AuditLogRepository.GetCountAsync(
                input.StartTime,
                input.EndTime,
                input.HttpMethod,
                input.Url,
                input.UserId,
                input.UserName,
                input.ApplicationName,
                input.ClientIpAddress,
                input.CorrelationId,
                input.MaxExecutionDuration,
                input.MinExecutionDuration,
                input.HasException,
                input.HttpStatusCode);

            var auditLogs = await AuditLogRepository.GetListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.StartTime,
                input.EndTime,
                input.HttpMethod,
                input.Url,
                input.UserId,
                input.UserName,
                input.ApplicationName,
                input.ClientIpAddress,
                input.CorrelationId,
                input.MaxExecutionDuration,
                input.MinExecutionDuration,
                input.HasException,
                input.HttpStatusCode);

            return new PagedResultDto<AuditLogDto>(
              count,
              ObjectMapper.Map<List<AuditLog>, List<AuditLogDto>>(auditLogs)
              );
        }

        /// <summary>
        /// EntityChange paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<EntityChangeDto>> GetEntityChangePagedListAsync(EntityChangePagedRequestDto input)
        {
            var count = await AuditLogRepository.GetEntityChangeCountAsync(
                input.AuditLogId,
                input.StartTime,
                input.EndTime,
                input.ChangeType,
                input.EntityId,
                input.EntityTypeFullName);

            var entityChanges = await AuditLogRepository.GetEntityChangeListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.AuditLogId,
                input.StartTime,
                input.EndTime,
                input.ChangeType,
                input.EntityId,
                input.EntityTypeFullName);

            return new PagedResultDto<EntityChangeDto>(
              count,
              ObjectMapper.Map<List<EntityChange>, List<EntityChangeDto>>(entityChanges)
              );
        }

        /// <summary>
        /// Get entityChange with username
        /// </summary>
        /// <param name="entityChangeId"></param>
        /// <returns></returns>
        public virtual async Task<EntityChangeWithUsernameDto> GetEntityChangeWithUsernameAsync(Guid entityChangeId)
        {
            var entityChangeWithUsername = await AuditLogRepository.GetEntityChangeWithUsernameAsync(entityChangeId);

            return ObjectMapper.Map<EntityChangeWithUsername, EntityChangeWithUsernameDto>(entityChangeWithUsername);
        }

        /// <summary>
        /// Get entityChanges with username
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityTypeFullName"></param>
        /// <returns></returns>
        public virtual async Task<List<EntityChangeWithUsernameDto>> GetEntityChangesWithUsernameAsync(string entityId, string entityTypeFullName)
        {
            var entityChangeWithUsernames = await AuditLogRepository.GetEntityChangesWithUsernameAsync(entityId, entityTypeFullName);

            return ObjectMapper.Map<List<EntityChangeWithUsername>, List<EntityChangeWithUsernameDto>>(entityChangeWithUsernames);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(AuditLoggingPermissions.AuditLogs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await AuditLogRepository.DeleteAsync(id);
        }
    }
}

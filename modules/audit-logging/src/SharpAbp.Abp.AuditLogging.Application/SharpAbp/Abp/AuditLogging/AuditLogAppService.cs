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
                input.CorrelationId,
                input.MaxExecutionDuration,
                input.MinExecutionDuration,
                input.HasException,
                input.HttpStatusCode);

            var auditLogs = await AuditLogRepository.GetListAsync(
                input.Sorting,
                input.SkipCount,
                input.MaxResultCount,
                input.StartTime,
                input.EndTime,
                input.HttpMethod,
                input.Url,
                input.UserId,
                input.UserName,
                input.ApplicationName,
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

    }
}

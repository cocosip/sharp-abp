using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.AuditLogging
{
    [RemoteService(Name = AuditLoggingRemoteServiceConsts.RemoteServiceName)]
    [Area("audit-logging")]
    [Route("api/audit-logging/audit-logs")]
    public class AuditLogController : AuditLoggingController, IAuditLogAppService
    {
        private readonly IAuditLogAppService _auditLogAppService;
        public AuditLogController(IAuditLogAppService auditLogAppService)
        {
            _auditLogAppService = auditLogAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<AuditLogDto> GetAsync(Guid id)
        {
            return await _auditLogAppService.GetAsync(id);
        }

        [HttpGet]
        public async Task<PagedResultDto<AuditLogDto>> GetPagedListAsync(AuditLogPagedRequestDto input)
        {
            return await _auditLogAppService.GetPagedListAsync(input);
        }

        [HttpGet]
        [Route("entity-changes")]
        public async Task<PagedResultDto<EntityChangeDto>> GetEntityChangePagedListAsync(EntityChangePagedRequestDto input)
        {
            return await _auditLogAppService.GetEntityChangePagedListAsync(input);
        }

        [HttpGet]
        [Route("entity-change-with-username/{entityChangeId}")]
        public async Task<EntityChangeWithUsernameDto> GetEntityChangeWithUsernameAsync(Guid entityChangeId)
        {
            return await _auditLogAppService.GetEntityChangeWithUsernameAsync(entityChangeId);
        }

        [HttpGet]
        [Route("entity-changes-with-username/{entityId}")]
        public async Task<List<EntityChangeWithUsernameDto>> GetEntityChangesWithUsernameAsync(string entityId, string entityTypeFullName)
        {
            return await _auditLogAppService.GetEntityChangesWithUsernameAsync(entityId, entityTypeFullName);
        }


    }
}

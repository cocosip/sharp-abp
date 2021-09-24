using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace SharpAbp.Abp.AuditLogging
{
    public class EntityChangePagedRequestDto : PagedAndSortedResultRequestDto
    {
        public Guid? AuditLogId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public EntityChangeType? ChangeType { get; set; }
        public string EntityId { get; set; }
        public string EntityTypeFullName { get; set; }
    }
}

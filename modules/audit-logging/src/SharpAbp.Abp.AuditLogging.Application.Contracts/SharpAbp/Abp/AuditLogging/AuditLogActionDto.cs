using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Data;

namespace SharpAbp.Abp.AuditLogging
{
    [DisableAuditing]
    public class AuditLogActionDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }

        public Guid AuditLogId { get; set; }

        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public string Parameters { get; set; }

        public DateTime ExecutionTime { get; set; }

        public int ExecutionDuration { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; set; }

        public AuditLogActionDto()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}

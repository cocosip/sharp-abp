using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Data;

namespace SharpAbp.Abp.AuditLogging
{
    [DisableAuditing]
    public class EntityChangeDto : EntityDto<Guid>
    {
        public Guid AuditLogId { get; set; }

        public Guid? TenantId { get; set; }

        public DateTime ChangeTime { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public Guid? EntityTenantId { get; set; }

        public string EntityId { get; set; }

        public string EntityTypeFullName { get; protected set; }

        public List<EntityPropertyChangeDto> PropertyChanges { get; set; }

        public ExtraPropertyDictionary ExtraProperties { get; set; }

        public EntityChangeDto()
        {
            PropertyChanges = new List<EntityPropertyChangeDto>();
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}

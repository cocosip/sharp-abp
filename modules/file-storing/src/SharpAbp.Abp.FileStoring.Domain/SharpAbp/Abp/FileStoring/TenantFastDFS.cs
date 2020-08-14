using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring
{
    public class TenantFastDFS : AuditedAggregateRoot<Guid>, IMultiTenant, ISoftDelete
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// The customer name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// FastDFS cluster id
        /// </summary>
        public Guid ClusterId { get; set; }

        /// <summary>
        /// FastDFS group id
        /// </summary>
        public Guid GroupId { get; set; }

        public bool IsDeleted { get; set; }
    }
}

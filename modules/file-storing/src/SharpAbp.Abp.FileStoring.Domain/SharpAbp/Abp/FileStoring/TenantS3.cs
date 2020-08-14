using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring
{
    public class TenantS3 : AuditedAggregateRoot<Guid>, IMultiTenant, ISoftDelete
    {
        public Guid? TenantId { get; set; }
        public virtual string Name { get; set; }

        /// <summary>
        /// S3 node id
        /// </summary>
        public Guid NodeId { get; set; }

        /// <summary>
        /// S3 bucket id
        /// </summary>
        public Guid BucketId { get; set; }


        public bool IsDeleted { get; set; }
    }
}

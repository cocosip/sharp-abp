using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.FileStoring
{
    public class S3Node : AuditedAggregateRoot<Guid>, ISoftDelete
    {
        /// <summary>
        /// S3 node name
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// S3 node server url
        /// </summary>
        public string ServerUrl { get; set; }

        public virtual List<S3Bucket> Buckets { get; set; }

        public bool IsDeleted { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.FileStoring
{
    public class FastDFSCluster : AuditedAggregateRoot<Guid>, ISoftDelete
    {
        /// <summary>
        /// FastDFS cluster name
        /// </summary>
        public virtual string ClusterName { get; set; }

        /// <summary>
        /// Use http access file
        /// </summary>
        public string HttpAccessUrl { get; set; }

        /// <summary>
        /// FastDFS tracker servers
        /// </summary>
        public virtual List<FastDFSTracker> Trackers { get; set; }

        /// <summary>
        /// FastDFS groups
        /// </summary>
        public virtual List<FastDFSGroup> Groups { get; set; }

        public bool IsDeleted { get; set; }
    }
}

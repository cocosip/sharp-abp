using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.FileStoring
{
    public class FastDFSGroup : Entity<Guid>
    {
        /// <summary>
        /// Relate FastDFS cluster id
        /// </summary>
        public Guid ClusterId { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        public string GroupName { get; set; }
    }
}

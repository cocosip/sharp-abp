using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.FileStoring
{
    public class FastDFSTracker : Entity<Guid>
    {
        /// <summary>
        /// Relate FastDFS cluster id
        /// </summary>
        public Guid ClusterId { get; set; }

        /// <summary>
        /// Tracker ip address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Tracker port
        /// </summary>
        public int Port { get; set; }
    }
}

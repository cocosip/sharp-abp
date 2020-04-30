using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.FileSystem
{
    public class FastDFSTracker : AggregateRoot<string>, IHasCreationTime, IHasModificationTime
    {
        /// <summary>Tracker名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>访问Url地址
        /// </summary>
        public string Url { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}

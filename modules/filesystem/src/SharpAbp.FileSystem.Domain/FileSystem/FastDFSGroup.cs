using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.FileSystem
{
    public class FastDFSGroup : AggregateRoot<string>, IHasCreationTime, IHasModificationTime
    {
        /// <summary>编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>状态
        /// </summary>
        public int State { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}

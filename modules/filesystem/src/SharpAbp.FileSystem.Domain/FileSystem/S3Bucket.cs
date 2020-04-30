using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.FileSystem
{
    public class S3Bucket : AggregateRoot<string>, IHasCreationTime, IHasModificationTime
    {
        /// <summary>医院编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>服务地址
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>AK
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>SK
        /// </summary>
        public string SecretAccessKey { get; set; }

        /// <summary>强制路径
        /// </summary>
        public int ForcePathStyle { get; set; }

        /// <summary>协议,0-HTTP,1-HTTPS
        /// </summary>
        public int Protocol { get; set; }

        /// <summary>供应商名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>供应商版本
        /// </summary>
        public string VendorVersion { get; set; }

        /// <summary>大文件分片大小
        /// </summary>
        public int SliceSize { get; set; }

        /// <summary>Bucket名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>服务版本
        /// </summary>
        public string SignatureVersion { get; set; }

        /// <summary>权限信息
        /// </summary>
        public int Acl { get; set; }

        /// <summary>访问地址
        /// </summary>
        public string AccessUrl { get; set; }

        /// <summary>状态
        /// </summary>
        public int State { get; set; }
        
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}

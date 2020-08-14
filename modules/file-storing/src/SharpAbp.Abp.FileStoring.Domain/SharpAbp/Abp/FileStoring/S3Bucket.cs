using System;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.FileStoring
{
    public class S3Bucket : Entity<Guid>
    {
        /// <summary>
        /// S3 node id
        /// </summary>
        public Guid S3NodeId { get; set; }

        /// <summary>
        /// Bucket name
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Server url(client sdk access url)
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// AccessKeyId
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// SecretAccessKey
        /// </summary>
        public string SecretAccessKey { get; set; }

        /// <summary>
        /// ForcePathStyle
        /// </summary>
        public bool ForcePathStyle { get; set; }

        /// <summary>
        /// Protocol,1-Http,2-Https
        /// </summary>
        public int Protocol { get; set; }

        /// <summary>
        /// UseChunkEncoding (Aliyun oss should set the to 'true')
        /// </summary>
        public bool UseChunkEncoding { get; set; }

        /// <summary>
        /// Vendor name (If use Kingsoft360,should use ks3 sdk)
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// VendorVersion (Maybe we should use difference sdk version)
        /// </summary>
        public string VendorVersion { get; set; }

        /// <summary>
        /// To slice upload file data when file is big
        /// </summary>
        public int SliceSize { get; set; }

        /// <summary>
        /// SignatureVersion
        /// </summary>
        public string SignatureVersion { get; set; }

        /// <summary>
        /// To access url
        /// </summary>
        public string AccessUrl { get; set; }
    }
}

using System;

namespace SharpAbp.FileSystem.S3
{
    /// <summary>S3存储的桶信息
    /// </summary>
    [Serializable]
    public class BucketInfo
    {
        /// <summary>服务地址
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>AK
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>SK
        /// </summary>
        public string SecretAccessKey { get; set; }

        private S3Vendor _vendor;

        /// <summary>S3供应商信息
        /// </summary>
        public S3Vendor Vendor
        {
            get
            {
                if (_vendor == null)
                {
                    _vendor = new S3Vendor(VendorName, VendorVersion);
                }
                return _vendor;
            }
        }

        /// <summary>供应商
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>供应商版本
        /// </summary>
        public string VendorVersion { get; set; }

        /// <summary>Bucket名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>S3协议类型
        /// </summary>
        public int S3Protocol { get; set; }

        /// <summary>是否强制路径重定向
        /// </summary>
        public bool ForcePathStyle { get; set; }

        /// <summary>大文件上传分片的大小(默认5MB)
        /// </summary>
        public int SliceSize { get; set; } = 5 * 1024 * 1024;

        /// <summary>服务版本
        /// </summary>
        public string SignatureVersion { get; set; }

        /// <summary>权限信息(1-私有,2-共有读,4-共有读和写,8-授权读)
        /// </summary>
        public int Acl { get; set; }

        /// <summary>访问地址
        /// </summary>
        public string AccessUrl { get; set; }

        /// <summary>Ctor
        /// </summary>
        public BucketInfo()
        {

        }
        public override string ToString()
        {
            return $"Bucket信息:[ServerUrl:{ServerUrl},AccessKeyId:{AccessKeyId},SecretAccessKey:{SecretAccessKey},Name:{Name},Title:{Title},Acl:{Acl},S3Protocol:{S3Protocol},ForcePathStyle:{ForcePathStyle},AccessUrl:{AccessUrl}]";
        }
    }
}

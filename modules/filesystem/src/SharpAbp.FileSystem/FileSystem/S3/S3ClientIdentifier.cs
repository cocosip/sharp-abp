using System;

namespace SharpAbp.FileSystem.S3
{
    /// <summary>S3客户端唯一标志
    /// </summary>
    public class S3ClientIdentifier : IEquatable<S3ClientIdentifier>
    {
        /// <summary>AK
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>SK
        /// </summary>
        public string SecretAccessKey { get; set; }

        /// <summary>供应商
        /// </summary>
        public S3Vendor Vendor { get; set; }

        /// <summary>Ctor
        /// </summary>
        public S3ClientIdentifier()
        {

        }

        public S3ClientIdentifier(string accessKeyId, string secretAccessKey, S3Vendor vendor)
        {
            AccessKeyId = accessKeyId;
            SecretAccessKey = secretAccessKey;
            Vendor = vendor;
        }


        /// <summary>判断对象是否相等
        /// </summary>
        public bool Equals(S3ClientIdentifier other)
        {
            return (other != null) && other.AccessKeyId == other.AccessKeyId && SecretAccessKey == other.SecretAccessKey && Vendor == other.Vendor;
        }

        /// <summary>重写相等方法
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return obj is S3ClientIdentifier && Equals((S3ClientIdentifier) obj);
        }

        /// <summary>重写获取HashCode方法
        /// </summary>
        public override int GetHashCode()
        {
            return (StringComparer.InvariantCulture.GetHashCode(AccessKeyId) & StringComparer.InvariantCulture.GetHashCode(SecretAccessKey) & Vendor.GetHashCode());
        }
    }
}

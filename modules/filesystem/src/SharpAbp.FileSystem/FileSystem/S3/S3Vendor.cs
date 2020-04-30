using System;

namespace SharpAbp.FileSystem.S3
{
    /// <summary>S3供应商
    /// </summary>
    public class S3Vendor : IEquatable<S3Vendor>
    {
        /// <summary>对象存储名
        /// </summary>
        public string Name { get; set; }

        /// <summary>版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>Ctor
        /// </summary>
        public S3Vendor()
        {

        }

        /// <summary>Ctor
        /// </summary>
        public S3Vendor(string name, string version)
        {
            Name = name;
            Version = version;
        }

        /// <summary>KS3默认名称
        /// </summary>
        public const string KS3Name = "KingSoft360";

        /// <summary>金山云
        /// </summary>
        public static S3Vendor KS3 = new S3Vendor(KS3Name, "1.0");

        /// <summary>判断是否相等
        /// </summary>
        public bool Equals(S3Vendor other)
        {
            return (other != null) && Name == other.Name && Version == other.Version;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return obj is S3Vendor && Equals((S3Vendor) obj);
        }

        public override int GetHashCode()
        {
            return (StringComparer.InvariantCulture.GetHashCode(Name) + StringComparer.InvariantCulture.GetHashCode(Version));
        }


    }
}

using System;

namespace SharpAbp.FileSystem
{
    /// <summary>Patch信息
    /// </summary>
    [Serializable]
    public abstract class Patch
    // : IEquatable<Patch>
    {
        /// <summary>存储类型
        /// </summary>
        public virtual int StoreType { get; set; }

        /// <summary>Patch名(OSS中为Bucket名,FastDFS中为组名,NFS中为服务器的信息)
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>编码
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>状态
        /// </summary>
        public int State { get; set; }

        // public bool Equals(Patch other)
        // {
        //     return (other != null) && Name == other.Name && StoreType == other.StoreType;
        // }

        // /// <summary>重写相等方法
        // /// </summary>
        // public override bool Equals(object obj)
        // {
        //     if (obj is null)
        //     {
        //         return false;
        //     }
        //     return obj is Patch && Equals((Patch) obj);
        // }

        // /// <summary>重写获取HashCode方法
        // /// </summary>
        // public override int GetHashCode()
        // {
        //     return StringComparer.InvariantCulture.GetHashCode(Name);
        // }
    }
}

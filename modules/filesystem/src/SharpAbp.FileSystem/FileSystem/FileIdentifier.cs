using System;

namespace SharpAbp.FileSystem
{
    /// <summary>文件唯一标志
    /// </summary>
    public class FileIdentifier : IEquatable<FileIdentifier>
    {
        /// <summary>存储类型
        /// </summary>
        public StoreType StoreType { get; set; }

        /// <summary>文件唯一标志(在S3中为文件的Key,FastDFS中为FileId,NFS中为存储路径地址)
        /// </summary>
        public string FileId { get; set; }

        /// <summary>Patch信息
        /// </summary>
        public Patch Patch { get; set; }

        /// <summary>Ctor
        /// </summary>
        public FileIdentifier()
        {

        }

        /// <summary>判断对象是否相等
        /// </summary>
        public bool Equals(FileIdentifier other)
        {
            return StoreType == other.StoreType && FileId == other.FileId
                && Patch == other.Patch;
        }

        /// <summary>重写相等方法
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return obj is FileIdentifier && Equals((FileIdentifier) obj);
        }

        /// <summary>重写获取HashCode方法
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(FileId) & StoreType.GetHashCode() & Patch.GetHashCode();
        }

        /// <summary>重写ToString方法
        /// </summary>
        public override string ToString()
        {
            return $"[存储类型:{StoreType.ToString()},PatchName:{Patch.Name},FileId:{FileId}]";
        }
    }
}

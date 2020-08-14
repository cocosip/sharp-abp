using System;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// The identity of file
    /// </summary>
    public class FileIdentity : IEquatable<FileIdentity>
    {
        /// <summary>
        /// TenantId
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// File provider name
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// File store container id
        /// </summary>
        public Guid ContainerId { get; set; }

        /// <summary>
        /// File store id
        /// </summary>
        public string FileId { get; set; }

        public bool Equals(FileIdentity other)
        {
            return TenantId == other.TenantId && ProviderName == other.ProviderName && ContainerId == other.ContainerId && FileId == other.FileId;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return obj is FileIdentity && Equals((FileIdentity)obj);
        }

        public override int GetHashCode()
        {
            return TenantId?.GetHashCode() ?? 0 & StringComparer.InvariantCulture.GetHashCode(ProviderName) & ContainerId.GetHashCode() & StringComparer.InvariantCulture.GetHashCode(FileId);
        }


        public override string ToString()
        {
            return $"TenantId:{TenantId},ProviderName:{ProviderName},ContainerId:{ContainerId},FileId:{FileId}";
        }
    }
}

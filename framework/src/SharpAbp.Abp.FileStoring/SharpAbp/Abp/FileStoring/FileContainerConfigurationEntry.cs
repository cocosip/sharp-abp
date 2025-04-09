using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerConfigurationEntry
    {
        public string? Provider { get; set; }
        public bool IsMultiTenant { get; set; }

        /// <summary>
        /// Enable auto multi-part upload
        /// </summary>
        /// <value></value>
        public bool EnableAutoMultiPartUpload { get; set; }

        /// <summary>
        /// Multi-part upload min file size
        /// </summary>
        /// <value></value>
        public int MultiPartUploadMinFileSize { get; set; }

        /// <summary>
        /// Multi-part upload sharding size
        /// </summary>
        /// <value></value>
        public int MultiPartUploadShardingSize { get; set; }
        public bool HttpAccess { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public FileContainerConfigurationEntry()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}

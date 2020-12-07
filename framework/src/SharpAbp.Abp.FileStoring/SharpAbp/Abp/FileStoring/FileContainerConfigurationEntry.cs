using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerConfigurationEntry
    {
        public string Provider { get; set; }
        public bool IsMultiTenant { get; set; }
        public bool HttpAccess { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public FileContainerConfigurationEntry()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}

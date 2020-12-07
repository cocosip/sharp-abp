using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoring
{
    public class ProviderConfiguration
    {
        public string Provider { get; set; }
        public bool IsMultiTenant { get; set; }
        public bool HttpAccess { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public ProviderConfiguration()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}

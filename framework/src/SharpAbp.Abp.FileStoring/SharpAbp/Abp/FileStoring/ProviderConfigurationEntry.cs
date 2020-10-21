using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoring
{
    public class ProviderConfigurationEntry
    {
        public string Provider { get; set; }

        public bool IsMultiTenant { get; set; }

        public bool HttpSupport { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public ProviderConfigurationEntry()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}

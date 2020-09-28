namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableDiscoveryProviderConfiguration
    {
        /// <summary>
        /// OverrideException
        /// </summary>
        public bool OverrideException
        {
            get => _discoveryConfiguration.GetConfigurationOrDefault(AddressTableDiscoveryProviderConfigurationNames.OverrideException, false);
            set => _discoveryConfiguration.SetConfiguration(AddressTableDiscoveryProviderConfigurationNames.OverrideException, value);
        }


        private readonly DiscoveryConfiguration _discoveryConfiguration;

        public AddressTableDiscoveryProviderConfiguration(DiscoveryConfiguration discoveryConfiguration)
        {
            _discoveryConfiguration = discoveryConfiguration;
        }
    }
}

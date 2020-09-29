using System;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class ConsulDiscoveryProviderConfiguration
    {

        /// <summary>
        /// Consul name
        /// </summary>
        public string ConsulName
        {
            get => _discoveryConfiguration.GetConfiguration<string>(ConsulDiscoveryProviderConfigurationNames.ConsulName);
            set => _discoveryConfiguration.SetConfiguration(ConsulDiscoveryProviderConfigurationNames.ConsulName, value.IsNullOrWhiteSpace() ? "" : value);
        }

        /// <summary>
        ///  Expired seconds
        /// </summary>
        public int ExpiredSeconds
        {
            get => _discoveryConfiguration.GetConfigurationOrDefault(ConsulDiscoveryProviderConfigurationNames.ExpiredSeconds, 30);
            set => _discoveryConfiguration.SetConfiguration(ConsulDiscoveryProviderConfigurationNames.ExpiredSeconds, value);
        }


        private readonly ServiceDiscoveryConfiguration _discoveryConfiguration;

        public ConsulDiscoveryProviderConfiguration(ServiceDiscoveryConfiguration discoveryConfiguration)
        {
            _discoveryConfiguration = discoveryConfiguration;
        }


    }
}

using SharpAbp.Abp.Consul;
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
            set => _discoveryConfiguration.SetConfiguration(ConsulDiscoveryProviderConfigurationNames.ConsulName, value.IsNullOrWhiteSpace() ? DefaultConsul.Name : value);
        }

        private readonly DiscoveryConfiguration _discoveryConfiguration;

        public ConsulDiscoveryProviderConfiguration(DiscoveryConfiguration discoveryConfiguration)
        {
            _discoveryConfiguration = discoveryConfiguration;
        }


    }
}

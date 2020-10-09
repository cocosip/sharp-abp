using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class AbpMicroDiscoveryOptions
    {
        public DiscoveryConfigurations Configurations { get; }
        public DiscoveryProviderNameMappers ProviderNameMappers { get; }

        public AbpMicroDiscoveryOptions()
        {
            Configurations = new DiscoveryConfigurations();
            ProviderNameMappers = new DiscoveryProviderNameMappers();
        }

        /// <summary>
        /// Configure the options from configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public AbpMicroDiscoveryOptions Configure(IConfiguration configuration)
        {
            var dict = configuration.Get<Dictionary<string, string>>();

            foreach (var kv in dict)
            {
                var providerType = ProviderNameMappers.GetProviderType(kv.Value);
                if (providerType == null)
                {
                    throw new AbpException($"Could not find provider type mapper by provider name {kv.Value}.");
                }

                if (kv.Key.Equals(DefaultDiscovery.Name, StringComparison.OrdinalIgnoreCase))
                {
                    Configurations.ConfigureDefault(c =>
                    {
                        c.ProviderType = providerType;
                    });
                }
                else
                {
                    Configurations.Configure(kv.Key, c =>
                    {
                        c.ProviderType = providerType;
                    });
                }
            }

            return this;
        }

    }

}

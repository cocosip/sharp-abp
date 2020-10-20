using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class AbpMicroLoadBalancerOptions
    {
        public LoadBalancerConfigurations Configurations { get; }

        public LoadBalancerProviderConfigurations Providers { get; }

        public AbpMicroLoadBalancerOptions()
        {
            Configurations = new LoadBalancerConfigurations();
            Providers = new LoadBalancerProviderConfigurations();
        }

        public AbpMicroLoadBalancerOptions Configure(IConfiguration configuration)
        {
            var loadBalancerConfigurationEntries = configuration.Get<Dictionary<string, LoadBalancerConfigurationEntry>>();

            foreach (var kv in loadBalancerConfigurationEntries)
            {
                var loadBalancerProviderConfiguration = Providers.GetConfiguration(kv.Value.BalancerType);
                if (loadBalancerProviderConfiguration == null)
                {
                    throw new AbpException($"Could not find any provider configuration for '{kv.Key}' loadbalancer, balancerType:'{kv.Value.BalancerType}'");
                }

                Configurations.Configure(kv.Key, c =>
                {
                    c.BalancerType = loadBalancerProviderConfiguration.BalancerType;

                    var defaultProperties = loadBalancerProviderConfiguration.GetProperties();

                    foreach (var defaultProperty in defaultProperties)
                    {
                        kv.Value.Properties.TryGetValue(defaultProperty.Key, out string value);
                        var realValue = TypeHelper.ConvertFromString(defaultProperty.Value, value);
                        c.SetConfiguration(defaultProperty.Key, realValue);
                    }
                });

            }
            return this;
        }

    }
}

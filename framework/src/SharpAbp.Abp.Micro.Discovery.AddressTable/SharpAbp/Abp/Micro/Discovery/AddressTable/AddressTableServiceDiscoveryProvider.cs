using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IAddressTableConfigurationSelector ConfigurationSelector { get; }

        public AddressTableServiceDiscoveryProvider(ILogger<AddressTableServiceDiscoveryProvider> logger, IAddressTableConfigurationSelector configurationSelector)
        {
            Logger = logger;
            ConfigurationSelector = configurationSelector;
        }

        public virtual Task<List<MicroService>> GetAsync([NotNull] string service, string tag = "", CancellationToken cancellationToken = default)
        {
            var configuration = ConfigurationSelector.Get(service);
            if (configuration == null)
            {
                Logger.LogDebug("Can't find address table configuration by service :{0}", service);
                return Task.FromResult(new List<MicroService>());
            }

            var services = ParseToService(configuration);
            services = FilterByTag(services, tag);
            return Task.FromResult(services);
        }


        private List<MicroService> ParseToService(AddressTableConfiguration configuration)
        {
            var services = new List<MicroService>();
            if (configuration.Entries != null && configuration.Entries.Any())
            {
                foreach (var entry in configuration.Entries)
                {
                    var service = new MicroService()
                    {
                        Id = entry.Id,
                        Service = configuration.Service,
                        Scheme = entry.Scheme,
                        Host = entry.Host,
                        Port = entry.Port,
                        Tags = entry.Tags
                    };
                    services.Add(service);
                }
            }
            return services;
        }

        private List<MicroService> FilterByTag(List<MicroService> services, string tag)
        {
            if (!tag.IsNullOrWhiteSpace())
            {
                return services.Where(x => x.Tags.Contains(tag)).ToList();
            }

            return services;
        }

    }
}

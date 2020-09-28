using Consul;
using SharpAbp.Abp.Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class ConsulServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {
        protected IConsulClientFactory ConsulClientFactory { get; }
        protected IDistributedCache<MicroServiceCacheItem> Cache { get; }

        public ConsulServiceDiscoveryProvider(
            IConsulClientFactory consulClientFactory,
            IDistributedCache<MicroServiceCacheItem> cache)
        {
            ConsulClientFactory = consulClientFactory;
            Cache = cache;
        }

        public virtual async Task<List<MicroService>> GetAsync(ServiceDiscoveryProviderGetArgs args)
        {
            var key = FormatCacheKey(args.Service);

            var microServiceCacheItem = await Cache.GetAsync(key);
            if (microServiceCacheItem == null)
            {

            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get consul client from ConsulClientFactory
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected virtual IConsulClient GetConsulClient(DiscoveryConfiguration configuration)
        {
            var consulDiscoveryConfiguration = configuration.GetConsulDiscoveryConfiguration();

            var client = ConsulClientFactory.Get(consulDiscoveryConfiguration.ConsulName);

            return client;
        }


        protected virtual string FormatCacheKey(string service)
        {
            return $"discovery-{service}";
        }

    }
}

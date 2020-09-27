using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class ConsulServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {

        private readonly IDistributedCache<List<MicroService>> _serviceCache;

        public ConsulServiceDiscoveryProvider(IDistributedCache<List<MicroService>> serviceCache)
        {
            _serviceCache = serviceCache;
        }

        public async Task<List<MicroService>> GetAsync(ServiceDiscoveryProviderGetArgs args)
        {
            var key = $"{"discovery"}#{args.Service}";
            var microServices = await _serviceCache.GetAsync(key);



            throw new NotImplementedException();
        }




    }
}

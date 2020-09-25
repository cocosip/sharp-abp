using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class ConsulServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {



        public Task<List<MicroService>> GetAsync(ServiceDiscoveryProviderGetArgs args)
        {
            throw new NotImplementedException();
        }
    }
}

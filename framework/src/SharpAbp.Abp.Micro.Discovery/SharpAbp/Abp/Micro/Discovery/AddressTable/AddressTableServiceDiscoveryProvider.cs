using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {



        public Task<List<MicroService>> GetAsync(ServiceDiscoveryProviderGetArgs args)
        {
            throw new NotImplementedException();
        }
    }
}

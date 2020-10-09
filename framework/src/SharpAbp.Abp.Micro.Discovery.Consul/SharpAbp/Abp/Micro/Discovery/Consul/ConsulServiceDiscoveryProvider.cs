using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class ConsulServiceDiscoveryProvider : IServiceDiscoveryProvider, ITransientDependency
    {



        public Task<List<MicroService>> GetAsync(string service, string tag = "", CancellationToken cancellationToken = default)
        {

            throw new System.NotImplementedException();
        }
    }
}

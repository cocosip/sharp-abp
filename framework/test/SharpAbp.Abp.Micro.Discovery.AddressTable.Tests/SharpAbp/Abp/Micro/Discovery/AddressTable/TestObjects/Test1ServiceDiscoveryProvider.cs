using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable.TestObjects
{
    public class Test1ServiceDiscoveryProvider : IServiceDiscoveryProvider
    {
        public Task<List<MicroService>> GetAsync(string service, string tag = "", CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<MicroService>());
        }
    }
}

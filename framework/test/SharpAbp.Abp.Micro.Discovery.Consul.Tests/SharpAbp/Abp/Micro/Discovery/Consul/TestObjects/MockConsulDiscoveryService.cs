using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.Discovery.Consul.TestObjects
{
    public class MockConsulDiscoveryService : IConsulDiscoveryService
    {
        public Task<List<MicroService>> GetAsync([NotNull] string service, CancellationToken cancellationToken = default)
        {
            var services = new List<MicroService>()
            {
                new MicroService()
                {

                }
            };
            return Task.FromResult(services);
        }
    }
}
